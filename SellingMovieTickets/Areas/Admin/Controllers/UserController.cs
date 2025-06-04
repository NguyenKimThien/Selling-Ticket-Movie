using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.Users;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;

        public UserController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAccount loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginVM.Username);
                if (user == null)
                {
                    TempData["Error"] = "Tài khoản không tồn tại.";
                    return View(loginVM);
                }

                var result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    if (!await _userManager.IsInRoleAsync(user, "Customer"))
                    {
                        // Lấy avatar, nếu không có thì sử dụng avatar mặc định
                        var avatar = user.Avatar ?? "avatar_default.jpg";
                        var roles = await _userManager.GetRolesAsync(user);
                        var role = roles.FirstOrDefault() ?? "Unknown";
                        var email = await _userManager.GetEmailAsync(user);

                        var claims = new List<Claim>
                                {
                                    new Claim(ClaimUserLogin.Id, user.Id),
                                    new Claim(ClaimUserLogin.Avatar, avatar),
                                    new Claim(ClaimUserLogin.FullName, user.FullName ?? user.UserName),
                                    new Claim(ClaimUserLogin.Email, email),
                                    new Claim(ClaimUserLogin.Role, role)
                                };

                        // Tạo claims identity và đăng nhập
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, // Cookie tồn tại sau khi đóng trình duyệt
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                        };
                        await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);

                        return RedirectToAction("Index", "HomeDashboard");
                    }
                    else
                    {
                        TempData["Error"] = "Bạn không có quyền truy cập vào khu vực này.";
                        await _signInManager.SignOutAsync(); // Đăng xuất nếu người dùng có quyền "Customer"
                        return View(loginVM);
                    }
                }
                else
                {
                    TempData["Error"] = "Password bị sai.";
                    return View(loginVM);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
            return View(loginVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl ?? Url.Action("Login", "User"));
        }

    }
}
