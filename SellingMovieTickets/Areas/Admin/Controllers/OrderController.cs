using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Order;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OrderDetail;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OrderDetailWithServices;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServicesOrder;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Room;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OrderController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<OrderModel> orders = await _context.Orders.OrderByDescending(x => x.Id).ToListAsync();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                orders = orders.Where(x =>
                  (x.OrderCode?.Contains(searchText) ?? false) ||
                  (x.PaymentType?.Contains(searchText) ?? false) ||
                  (x.NumberOfTickets == int.Parse(searchText)) ||
                  (x.TicketCode?.Contains(searchText) ?? false) ||
                  (x.TotalAmount == decimal.Parse(searchText)));
            }

            int recsCount = orders.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = orders.Skip(resSkip).Take(pager.PageSize).ToList();

            var ordersVM = _mapper.Map<List<OrderViewModel>>(orders);
            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(ordersVM);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == id).ToListAsync();
            var orderDetailsVM = _mapper.Map<List<OrderDetailViewModel>>(orderDetails);

            var otherServicesOrders = await _context.OtherServicesOrders.Where(os => os.OrderId == id).Include(os => os.OtherServices).ToListAsync();
            var otherServiceOrdersVM = _mapper.Map<List<OtherServicesOrderViewModel>>(otherServicesOrders);

            if (orderDetails == null || otherServicesOrders == null)
            {
                return NotFound();
            }

            var model = new OrderDetailWithServices
            {
                orderDetailVM = orderDetailsVM,
                otherServicesOrderVM = otherServiceOrdersVM
            };

            return View(model);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            OrderModel order = await _context.Orders.FindAsync(Id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa hóa đơn thành công";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy hóa đơn";
            }
            return RedirectToAction("Index");
        }

    }
}
