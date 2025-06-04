using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Accounts;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.User;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using SellingMovieTickets.Services.Interfaces;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private readonly IAwsS3Service _awsS3Service;

        public AccountController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext, IAwsS3Service awsS3Service)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
            _awsS3Service = awsS3Service;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            var userRoles = await (from u in _dataContext.Users
                                   join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                   join r in _dataContext.Roles on ur.RoleId equals r.Id
                                   select new { User = u, Role = r.Name }).ToListAsync();

            IEnumerable<UserViewModel> responseAccounts = userRoles
                                  .GroupBy(ur => ur.User)
                                  .Select(group => new UserViewModel
                                  {
                                      Id = group.Key.Id,
                                      UserName = group.Key.UserName,
                                      Email = group.Key.Email,
                                      FullName = group.Key.FullName,
                                      Gender = group.Key.Gender,
                                      Avatar = group.Key.Avatar,
                                      PhoneNumber = group.Key.PhoneNumber,
                                      CreateBy = group.Key.CreateBy,
                                      CreateDate = group.Key.CreateDate,
                                      ModifiedBy = group.Key.ModifiedBy,
                                      ModifiedDate = group.Key.ModifiedDate.ToString(),
                                      Role = group.Select(x => x.Role).FirstOrDefault()
                                  }).ToList();


            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                responseAccounts = responseAccounts.Where(x =>
                   (x.UserName?.Contains(searchText) ?? false) ||
                   (x.PhoneNumber?.Contains(searchText) ?? false) ||
                   (x.Email?.Contains(searchText) ?? false) ||
                   (x.Role?.Contains(searchText) ?? false));
            }

            int recsCount = responseAccounts.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = responseAccounts.Skip(resSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            var model = new CreateAccount();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccount model)
        {
            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                var emailEditor = User.FindFirstValue(ClaimUserLogin.Email);

                var findUserName = await _userManager.FindByNameAsync(model.User.Username);
                var findEmail = await _userManager.FindByEmailAsync(model.User.Email);
                if (findUserName != null)
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    TempData["Error"] = "Username đã tồn tại.";
                    return View(model);
                }
                else if (findEmail != null)
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    TempData["Error"] = "Email đã tồn tại.";
                    return View(model);
                }

                var user = new AppUserModel
                {
                    UserName = model.User.Username,
                    Email = model.User.Email,
                    PasswordHash = model.User.Password,
                    PhoneNumber = model.User.PhoneNumber,
                    FullName = model.User.FullName,
                    Gender = model.User.Gender,
                    CreateDate = DateTime.Now,
                    CreateBy = nameEditor,
                    ModifiedDate = DateTime.Now,
                };

                if (model.User.ImageUpload != null)
                {
                    var resultUpload = await _awsS3Service.UploadFile(model.User.ImageUpload, "account", model.User.ImageUpload.FileName);
                    if (resultUpload.StatusCode == 200)
                    {
                        user.Avatar = resultUpload.PresignedUrl;
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi: " + string.Join(", ", resultUpload.Message);
                        return View(model);
                    }
                }

                var result = await _userManager.CreateAsync(user, model.User.Password);
                if (result.Succeeded)
                {
                    var selectedRole = model.Role != null ? model.Role : Role.Customer;
                    var role = await _roleManager.FindByIdAsync(selectedRole);

                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else
                    {
                        var roleName = await _roleManager.FindByNameAsync(selectedRole);
                        if (roleName != null)
                        {
                            await _userManager.AddToRoleAsync(user, roleName.Name);
                        }
                    }

                    CustomerManagementModel customerManagement = new CustomerManagementModel
                    {
                        UserId = user.Id,
                        CreateDate = DateTime.Now
                    };
                    await _dataContext.CustomerManagements.AddAsync(customerManagement);
                    await _dataContext.SaveChangesAsync();

                    TempData["Success"] = "Tạo tài khoản thành công.";
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                    return View(model);
                }
            }
            else
            {
                var roles = await _roleManager.Roles.ToListAsync();
                ViewBag.Roles = new SelectList(roles, "Id", "Name");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var model = new UpdateAccount
            {
                User = new UpdateUser
                {
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    Gender = user.Gender,
                    Avatar = user.Avatar
                },
                Role = userRole
            };

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, UpdateAccount model)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var user = await _userManager.FindByIdAsync(Id);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (model.User.ImageUpload == null)
            {
                model.User.Avatar = user.Avatar;
            }

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.UserName = model.User.Username;
                    user.Email = model.User.Email;
                    user.PhoneNumber = model.User.PhoneNumber;
                    user.FullName = model.User.FullName;
                    user.Gender = model.User.Gender;
                    user.ModifiedBy = nameEditor;
                    user.ModifiedDate = DateTime.Now;

                    if (model.User.ImageUpload != null)
                    {
                        if (user.Avatar != null)
                        {
                            var resultDelete = await _awsS3Service.DeleteFileAsync(user.Avatar);
                            if (resultDelete.StatusCode != 200)
                            {
                                TempData["Error"] = "Lỗi: " + string.Join(", ", resultDelete.Message);
                                return View(model);
                            }
                        }
                        var resultCreate = await _awsS3Service.UploadFile(model.User.ImageUpload, "account", model.User.ImageUpload.FileName);
                        if (resultCreate.StatusCode == 200)
                        {
                            user.Avatar = resultCreate.PresignedUrl;
                        }
                        else
                        {
                            TempData["Error"] = resultCreate.Message;
                            return View(model);
                        }
                    }

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        if (model.Role != null)
                        {
                            var currentRoles = await _userManager.GetRolesAsync(user);
                            var selectedRole = model.Role;
                            // xóa vai trò hiện tại
                            if (currentRoles.Any())
                            {
                                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            }

                            var role = await _roleManager.FindByNameAsync(selectedRole);
                            if (role != null)
                            {
                                await _userManager.AddToRoleAsync(user, role.Name);
                            }
                        }

                        TempData["Success"] = "Cập nhật tài khoản thành công.";
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        TempData["Error"] = "Cập nhật tài khoản không thành công.";
                        model.Role = userRole;
                        return View(model);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Name", "Name");
                    model.Role = userRole;
                    return View(model);
                }
            }
            else
            {
                var roles = await _roleManager.Roles.ToListAsync();
                ViewBag.Roles = new SelectList(roles, "Name", "Name");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                model.Role = userRole;
                return View(model);
            }
        }


        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Avatar != null)
            {
                var resultDelete = await _awsS3Service.DeleteFileAsync(user.Avatar);
                if (resultDelete.StatusCode != 200)
                {
                    TempData["Error"] = resultDelete.Message;
                    return RedirectToAction("Index");
                }
            }
            await _userManager.DeleteAsync(user);
            await _dataContext.SaveChangesAsync();
            TempData["Success"] = "User xóa thành công. ";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = _dataContext.Users.Find(item);
                        if (obj.Avatar != null)
                        {
                            await _awsS3Service.DeleteFileAsync(obj.Avatar);
                        }
                        _dataContext.Users.Remove(obj);
                        _dataContext.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
