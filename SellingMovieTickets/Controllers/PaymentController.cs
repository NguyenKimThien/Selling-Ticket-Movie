using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SellingMovieTickets.Areas.Admin.Services.Interfaces;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.CinemaShowTimes;
using SellingMovieTickets.Models.ViewModels.Payments;
using SellingMovieTickets.Models.ViewModels.VnPays;
using SellingMovieTickets.Repository;
using SellingMovieTickets.Services.Interfaces;
using System.Drawing;
using System.Drawing.Printing;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SellingMovieTickets.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DataContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailSender _emailSender;

        public PaymentController(DataContext context, IVnPayService vnPayService, IEmailSender emailSender)
        {
            _context = context;
            _vnPayService = vnPayService;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] PaymentInfo paymentInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await CheckAvailableSeatsAsync(paymentInfo.OrderSeats);
                    if (result)
                    {
                        var userName = User.FindFirstValue(ClaimUserLogin.FullName);
                        var userEmail = User.FindFirstValue(ClaimUserLogin.Email);

                        if (paymentInfo.PaymentType == PaymentType.VNPAY)
                        {
                            var userId = User.FindFirstValue(ClaimUserLogin.Id);
                            var customerManagement = await _context.CustomerManagements.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                            var showTime = await _context.CinemaShowTimes.Where(x => x.Id == paymentInfo.ShowTimeId).Include(x => x.Movie).Include(x => x.Room).FirstOrDefaultAsync();
                            var seatNames = string.Join(", ", paymentInfo.OrderSeats.Select(seatInfo => seatInfo.SeatName));

                            // tạo ticket
                            var ticket = new TicketModel
                            {
                                NameMovie = showTime.Movie.Name,
                                TicketCode = Guid.NewGuid().ToString(),
                                StartShowTime = showTime.StartShowTime,
                                ConcessionAmount = paymentInfo.ConcessionAmount,
                                DiscountAmount = paymentInfo.DiscountAmount,
                                TotalAmount = paymentInfo.PaymentAmount,
                                PaymentAmount = paymentInfo.PaymentAmount,
                                SeatNames = seatNames,
                                RoomNumber = showTime.Room.RoomNumber,
                                CreateDate = DateTime.Now,
                            };
                            await _context.Tickets.AddAsync(ticket);
                            await _context.SaveChangesAsync();

                            // tạo order
                            var order = new OrderModel();
                            order.OrderCode = Guid.NewGuid().ToString();
                            order.PaymentType = paymentInfo.PaymentType;
                            order.NumberOfTickets = paymentInfo.OrderSeats.Count();
                            order.TicketCode = ticket.TicketCode;
                            order.TotalAmount = paymentInfo.PaymentAmount;
                            order.StatusOrder = "";
                            order.TicketId = ticket.Id;
                            order.CinemaShowTimeId = paymentInfo.ShowTimeId;
                            order.CustomerManagementId = customerManagement.Id;
                            if (paymentInfo.PromotionId != null)
                            {
                                order.PromotionId = paymentInfo.PromotionId;
                            }
                            order.CreateDate = DateTime.Now;
                            await _context.Orders.AddAsync(order);
                            await _context.SaveChangesAsync();

                            var vnPayModel = new VnPaymentRequestModel
                            {
                                Amount = paymentInfo.PaymentAmount * 1000,
                                CreatedDate = DateTime.Now,
                                Description = $"{userName}",
                                FullName = userName,
                                OrderId = order.OrderCode
                            };

                            // tạo orderDetail
                            var orderDetails = paymentInfo.OrderSeats.Select(seatInfo => new OrderDetailModel
                            {
                                OrderId = order.Id,
                                OrderCode = order.OrderCode,
                                SeatNumber = seatInfo.SeatName,
                                Price = seatInfo.Price,
                                CreateDate = DateTime.Now
                            }).ToList();
                            await _context.OrderDetails.AddRangeAsync(orderDetails);
                            await _context.SaveChangesAsync();

                            if (paymentInfo.OtherServices != null)
                            {
                                var orderServiceOrders = new List<OtherServicesOrderModel>();
                                foreach (var service in paymentInfo.OtherServices)
                                {
                                    var otherService = await _context.OtherServices.FindAsync(service.Id);
                                    if (otherService != null)
                                    {
                                        var totalAmount = otherService.Price * service.Quantity;
                                        orderServiceOrders.Add(new OtherServicesOrderModel
                                        {
                                            OrderId = order.Id,
                                            OtherServicesId = service.Id,
                                            Quantity = service.Quantity,
                                            TotalAmount = totalAmount,
                                            CreateDate = DateTime.Now
                                        });
                                    }
                                }
                                await _context.OtherServicesOrders.AddRangeAsync(orderServiceOrders);
                                await _context.SaveChangesAsync();
                            }

                            var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
                            return Json(new { redirectUrl = paymentUrl, status = "Success" });
                        }
                    }
                    else
                    {
                        var responseUrl = @Url.Action("Index", "Home");
                        return Json(new { redirectUrl = responseUrl, status = "Fail" });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lưu thông tin thanh toán.", error = ex.Message, stackTrace = ex.StackTrace });
                }
            }
            return View(paymentInfo);
        }

        [Authorize]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Error"] = $"Lỗi thanh toán VNPay: {response.VnPayResponseCode}";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = User.FindFirstValue(ClaimUserLogin.Id);
                var userEmail = User.FindFirstValue(ClaimUserLogin.Email);
                var customerManagement = _context.CustomerManagements.Where(x => x.UserId == userId).FirstOrDefault();
                var order = _context.Orders.Include(o => o.Ticket).FirstOrDefault(o => o.OrderCode == response.OrderId);

                if (order == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin đơn hàng.";
                    return RedirectToAction("Index", "Home");
                }

                var ticket = order.Ticket;
                ticket.PaymentTime = response.PaymentDate;
                order.StatusOrder = response.VnPayResponseCode;
                _context.Tickets.Update(ticket);
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                var receiver = userEmail;
                var subject = $@"Thông tin đặt vé xem phim - Mã Thanh toán {order.OrderCode}";
                var message = GenerateTicketEmailContent(ticket, response.PaymentDate);

                _emailSender.SendEmailAsync(receiver, subject, message).Wait();

                var orderDetails = _context.OrderDetails
                    .Where(od => od.OrderId == order.Id).ToList();

                var seatsToUpdate = _context.Seats
                    .Where(seat => seat.CinemaShowTimeId == order.CinemaShowTimeId).ToList()
                    .Where(seat => orderDetails.Any(od => od.SeatNumber == seat.SeatNumber)).ToList();

                foreach (var seat in seatsToUpdate)
                {
                    seat.IsAvailable = false;
                }
                _context.Seats.UpdateRange(seatsToUpdate);
                await _context.SaveChangesAsync();

                // Cập nhật điểm thưởng
                customerManagement.TotalTicketsPurchased += order.NumberOfTickets;
                customerManagement.TotalSpent += order.TotalAmount;
                customerManagement.CurrentPointsBalance += 10;
                _context.CustomerManagements.Update(customerManagement);

                // Cập nhật lịch sử điểm thưởng
                var customerPointsHistory = new CustomerPointsHistoryModel
                {
                    CustomerId = customerManagement.Id,
                    PointsChanged = 10,
                    TransactionDate = DateTime.Now,
                    PointChangeStatus = PointChangeStatus.BuyTicket,
                    OrderId = order.Id,
                    CreateDate = DateTime.Now,
                };
                _context.CustomerPointsHistories.Add(customerPointsHistory);
                _context.SaveChanges();

                TempData["Success"] = $"Thanh toán VNPay thành công.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý sau thanh toán.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index", "Home");
            }
        }

        private string GenerateTicketEmailContent(TicketModel ticket, DateTime paymentDate)
        {
            var message = $@"
                 <!DOCTYPE html>
                 <html lang='en'>
                 <head>
                     <meta charset='UTF-8'>
                     <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                     <title>Movie Ticket</title>
                     <style>
                         body {{
                             font-family: Arial, sans-serif;
                             background-color: #f8f9fa;
                             color: #333;
                             padding: 20px;
                         }}

                         .ticket-container {{
                             max-width: 800px;
                             padding: 30px;
                             margin: 20px auto;
                             border: 1px solid #ddd;
                             border-radius: 10px;
                             background-color: #fff;
                             box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                             text-align: center;
                         }}

                         .header h2 {{
                             font-size: 24px;
                             font-weight: bold;
                             margin: 0;
                             color: #000;
                         }}

                         .header p {{
                             margin: 5px 0;
                             font-size: 14px;
                             color: #666;
                         }}

                         .header strong {{
                             font-size: 16px;
                             color: #007bff;
                         }}

                         .reservation-code {{
                             font-size: 28px;
                             font-weight: bold;
                             color: #e74c3c;
                             margin: 15px 0;
                         }}

                         .section-title {{
                             font-weight: bold;
                             font-size: 16px;
                             color: #333;
                             margin: 20px 0 10px;
                             text-transform: uppercase;
                         }}

                         .details-table {{
                             width: 100%;
                             border-collapse: collapse;
                             margin-top: 20px;
                             font-size: 14px;
                         }}

                         .details-table td {{
                             padding: 10px 15px;
                             border-bottom: 1px solid #ddd;
                         }}

                         .details-table .label {{
                             font-weight: bold;
                             color: #555;
                             text-align: left;
                             width: 50%;
                         }}

                         .details-table td:nth-child(2) {{
                             font-weight: bold;
                             color: #333;
                         }}

                         .footer {{
                             margin-top: 20px;
                             color: #666;
                             font-size: 12px;
                             line-height: 1.6;
                         }}

                         .footer p {{
                             margin: 5px 0;
                         }}

                         .footer p strong {{
                             color: #000;
                         }}
                     </style>
                 </head>
                 <body>
                     <div class='ticket-container'>
                         <div class='header'>
                             <img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTp1v7T287-ikP1m7dEUbs2n1SbbLEqkMd1ZA&s' alt='VNPay'>
                             <h2>{ticket.NameMovie}</h2>
                             <p><strong>Umi West Lake</strong></p>
                             <p>Tầng 4 Umi Mall West Lake Hanoi, 683 đường Lạc Long Quân, Tây Hồ, Hà Nội, Vietnam</p>
                         </div>

                         <p class='section-title'>Mã vé (Reservation Code)</p>
                         <p class='reservation-code'>{ticket.TicketCode}</p>

                         <p class='section-title'>Thông tin chi tiết</p>
                         <table class='details-table'>
                             <tr><td class='label'>Suất chiếu:</td><td>{ticket.StartShowTime:dd/MM/yyyy HH:mm}</td></tr>
                             <tr><td class='label'>Phòng chiếu:</td><td>{ticket.RoomNumber}</td></tr>
                             <tr><td class='label'>Ghế:</td><td>{ticket.SeatNames}</td></tr>
                             <tr><td class='label'>Thời gian thanh toán:</td><td>{paymentDate}</td></tr>
                             <tr><td class='label'>Tiền Combo bỏng nước:</td><td>{ticket.ConcessionAmount}VND</td></tr>
                             <tr><td class='label'>Tổng tiền:</td><td>{ticket.TotalAmount}VND</td></tr>
                             <tr><td class='label'>Số tiền giảm giá:</td><td>{ticket.DiscountAmount}VND</td></tr>
                             <tr><td class='label'>Số tiền thanh toán:</td><td>{ticket.PaymentAmount}VND</td></tr>
                         </table>

                         <div class='footer'>
                             <p>Quý khách vui lòng tới quầy dịch vụ xuất trình mã vé này để được nhận vé.</p>
                             <p>Xin cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                         </div>
                     </div>
                 </body>
                 </html>";
            return message;
        }

        private async Task<bool> CheckAvailableSeatsAsync(List<InforSeat> seats)
        {
            var seatIds = seats.Select(s => s.Id).ToList();
            var unavailableSeats = await _context.Seats.Where(s => seatIds.Contains(s.Id) && !s.IsAvailable).AnyAsync();  
            return !unavailableSeats;  
        }
    }
}
