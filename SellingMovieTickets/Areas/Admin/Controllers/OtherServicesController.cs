using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Adv;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.CinemaShowTime;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServices;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using SellingMovieTickets.Services.Interfaces;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OtherServicesController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAwsS3Service _awsS3Service;

        public OtherServicesController(DataContext context, IAwsS3Service awsS3Service, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _awsS3Service = awsS3Service;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<OtherServicesModel> otherServices = await _context.OtherServices.OrderByDescending(x => x.Id).ToListAsync();

            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                otherServices = otherServices.Where(x =>
                    (x.Name?.Contains(searchText) ?? false) ||
                    (x.Description?.Contains(searchText) ?? false) ||
                    (x.Image?.Contains(searchText) ?? false));
            }

            int recsCount = otherServices.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = otherServices.Skip(resSkip).Take(pager.PageSize).ToList();
            var otherServicesVM = _mapper.Map<List<OtherServicesViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(otherServicesVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOtherServices createOther)
        {
            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);

                var existingOrtherServices = await _context.OtherServices.FirstOrDefaultAsync(p => p.Name == createOther.Name);
                if (existingOrtherServices != null)
                {
                    TempData["Error"] = "Tên dịch vụ đã có trong cơ sở dữ liệu";
                    return View(createOther);
                }

                OtherServicesModel otherServices = new OtherServicesModel();
                otherServices.Name = createOther.Name;
                otherServices.Description = createOther.Description;
                otherServices.Price = createOther.Price;
                otherServices.Status = createOther.Status;
                otherServices.CreateDate = DateTime.Now;
                otherServices.CreateBy = nameEditor;
                otherServices.ModifiedDate = DateTime.Now;
                if (createOther.ImageUpload != null)
                {
                    var result = await _awsS3Service.UploadFile(createOther.ImageUpload, "otherservices", createOther.ImageUpload.FileName);
                    if (result.StatusCode == 200)
                    {
                        otherServices.Image = result.PresignedUrl;
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi: " + string.Join(", ", result.Message);
                        return View(createOther);
                    }
                }
                _context.Add(otherServices);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm dịch vụ thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(createOther);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var otherServices = await _context.OtherServices.FindAsync(id);
            if (otherServices == null)
            {
                TempData["Error"] = "Quảng cáo không tồn tại";
                return RedirectToAction("Index");
            }
            var updateOtherServices = MapToUpdateOtherServices(otherServices);
            return View(updateOtherServices);
        }

        private UpdateOtherServices MapToUpdateOtherServices(OtherServicesModel otherServices)
        {
            return new UpdateOtherServices
            {
                Name = otherServices.Name,
                Description = otherServices.Description,
                Price = otherServices.Price,
                Image = otherServices.Image,
                Status = otherServices.Status
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateOtherServices updateOther)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var existingOtherServices = await _context.OtherServices.FindAsync(id);
            if (updateOther.ImageUpload == null)
            {
                updateOther.Image = existingOtherServices.Image;
            }

            if (existingOtherServices == null)
            {
                TempData["Error"] = "Dịch vụ không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingOtherServices.Name = updateOther.Name;
                existingOtherServices.Description = updateOther.Description;
                existingOtherServices.Price = updateOther.Price;
                existingOtherServices.Status = updateOther.Status;
                existingOtherServices.ModifiedBy = nameEditor;
                existingOtherServices.ModifiedDate = DateTime.Now;

                if (updateOther.ImageUpload != null)
                {
                    if (existingOtherServices.Image != null)
                    {
                        var resultDelete = await _awsS3Service.DeleteFileAsync(existingOtherServices.Image);
                        if (resultDelete.StatusCode != 200)
                        {
                            TempData["Error"] = "Lỗi: " + string.Join(", ", resultDelete.Message);
                            return View(updateOther);
                        }
                    }
                    var resultCreate = await _awsS3Service.UploadFile(updateOther.ImageUpload, "otherservices", updateOther.ImageUpload.FileName);
                    if (resultCreate.StatusCode == 200)
                    {
                        existingOtherServices.Image = resultCreate.PresignedUrl;
                    }
                    else
                    {
                        TempData["Error"] = resultCreate.Message;
                        return View(updateOther);
                    }
                }

                _context.Update(existingOtherServices);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật dịch vụ thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(updateOther);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            OtherServicesModel otherServices = await _context.OtherServices.FindAsync(Id);
            if (otherServices.Image != null)
            {
                var resultDelete = await _awsS3Service.DeleteFileAsync(otherServices.Image);
                if (resultDelete.StatusCode != 200)
                {
                    TempData["Error"] = resultDelete.Message;
                    return RedirectToAction("Index");
                }
            }
            _context.OtherServices.Remove(otherServices);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa dịch vụ thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, Status status)
        {
            var otherServicesItem = _context.OtherServices.FirstOrDefault(n => n.Id == id);
            if (otherServicesItem == null)
            {
                return Json(new { success = false });
            }

            otherServicesItem.Status = status;
            _context.SaveChanges();
            return Json(new { success = true });
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
                        var obj = _context.OtherServices.Find(Convert.ToInt32(item));
                        if (obj.Image != null)
                        {
                            await _awsS3Service.DeleteFileAsync(obj.Image);
                        }
                        _context.OtherServices.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
