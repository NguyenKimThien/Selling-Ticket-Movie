using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.AccountManagement;
using SellingMovieTickets.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace SellingMovieTickets.Controllers
{
    public class CustomerManagementController : Controller
    {
        private DataContext _dataContext;
        private SignInManager<AppUserModel> _signInManager;

        public CustomerManagementController(DataContext dataContext, SignInManager<AppUserModel> signInManager)
        {
            _dataContext = dataContext;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var idCustomer = User.FindFirstValue(ClaimUserLogin.Id);
            var user = await _dataContext.Users.Where(x => x.Id == idCustomer).FirstOrDefaultAsync();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(AppUserModel model)
        {
            if (model != null)
            {
                var user = await _dataContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.FullName = model.LastName + " " + model.FirstName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();

                var claims = new List<Claim>
                                {
                                    new Claim(ClaimUserLogin.Id, user.Id),
                                    new Claim(ClaimUserLogin.Avatar, user.Avatar),
                                    new Claim(ClaimUserLogin.FullName, user.FullName),
                                    new Claim(ClaimUserLogin.Email, user.Email),
                                    new Claim(ClaimUserLogin.Role, User.FindFirstValue(ClaimUserLogin.Role))
                                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Cookie tồn tại sau khi đóng trình duyệt
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };
                await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);

                TempData["Success"] = "Cập nhật thông tin thành công.";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Cập nhật thông tin thất bại.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoHistoryTicket()
        {
            var idCustomer = User.FindFirstValue(ClaimUserLogin.Id);

            var tickets = await _dataContext.Orders
                .Where(o => o.CustomerManagement.UserId == idCustomer && o.StatusOrder == "00")
                .Include(o => o.OrderDetails)
                .Include(o => o.CinemaShowTime.Movie)
                .ToListAsync();

            var historyTickets = tickets.Select(order => new HistoryTicket
            {
                TradingDate = order.CreateDate,
                NameMovie = order.CinemaShowTime.Movie.Name,
                NumberOfTickets = order.OrderDetails.Count,
                TotalAmount = order.OrderDetails.Sum(detail => detail.Price)
            }).ToList();

            return Json(historyTickets);
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoHistoryPoint()
        {
            var idCustomer = User.FindFirstValue(ClaimUserLogin.Id);
            var historyPoints = await _dataContext.CustomerPointsHistories
                   .Include(x => x.Customer)
                   .Include(x => x.Order)
                   .Where(x => x.Customer.UserId == idCustomer)
                   .Select(x => new
                   {
                       x.Id,
                       x.PointsChanged,
                       x.TransactionDate,
                       x.PointChangeStatus,
                       x.Order.OrderCode,
                   })
                   .ToListAsync();
            return Json(historyPoints);
        }
    }
}
