using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SellingMovieTickets.Areas.Admin.Models.ViewModels;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Adv;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using SellingMovieTickets.Services.Implements;
using SellingMovieTickets.Services.Interfaces;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdvController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAwsS3Service _awsS3Service;

        public AdvController(DataContext context, IMapper mapper, IAwsS3Service awsS3Service)
        {
            _context = context;
            _mapper = mapper;
            _awsS3Service = awsS3Service;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<AdvModel> advs = await _context.Advs.OrderByDescending(x => x.Id).ToListAsync();

            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                advs = advs.Where(x =>
                    (x.Title?.Contains(searchText) ?? false) ||
                    (x.Description?.Contains(searchText) ?? false) ||
                    (x.Image?.Contains(searchText) ?? false) ||
                    (x.Link?.Contains(searchText) ?? false));
            }

            int recsCount = advs.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = advs.Skip(resSkip).Take(pager.PageSize).ToList();
            var advVM = _mapper.Map<List<AdvViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(advVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdvModel adv)
        {
            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                if (adv.ImageUpload != null)
                {
                    var result = await _awsS3Service.UploadFile(adv.ImageUpload, "adv", adv.ImageUpload.FileName);
                    if (result.StatusCode == 200)
                    {
                        adv.Image = result.PresignedUrl;
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi: " + string.Join(", ", result.Message);
                        return View(adv);
                    }
                }
                adv.CreateDate = DateTime.Now;
                adv.ModifiedDate = DateTime.Now;
                adv.CreateBy = nameEditor;
                _context.Add(adv);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm quảng cáo thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(adv);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var adv = await _context.Advs.FindAsync(id);
            if (adv == null)
            {
                TempData["Error"] = "Quảng cáo không tồn tại";
                return RedirectToAction("Index");
            }
            return View(adv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdvModel adv)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var existingAdv = await _context.Advs.FindAsync(id);
            if (adv.ImageUpload == null)
            {
                adv.Image = existingAdv.Image;
            }

            if (existingAdv == null)
            {
                TempData["Error"] = "Quảng cáo không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingAdv.Title = adv.Title;
                existingAdv.Description = adv.Description;
                existingAdv.Link = adv.Link;
                existingAdv.Status = adv.Status;
                existingAdv.ModifiedBy = nameEditor;
                existingAdv.ModifiedDate = DateTime.Now;

                if (adv.ImageUpload != null)
                {
                    if (existingAdv.Image != null)
                    {
                        var resultDelete = await _awsS3Service.DeleteFileAsync(existingAdv.Image);
                        if (resultDelete.StatusCode != 200)
                        {
                            TempData["Error"] = "Lỗi: " + string.Join(", ", resultDelete.Message);
                            return View(adv);
                        }
                    }
                    var resultCreate = await _awsS3Service.UploadFile(adv.ImageUpload, "adv", adv.ImageUpload.FileName);
                    if (resultCreate.StatusCode == 200)
                    {
                        existingAdv.Image = resultCreate.PresignedUrl;
                    }
                    else
                    {
                        TempData["Error"] = resultCreate.Message;
                        return View(adv);
                    }
                }
                _context.Update(existingAdv);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật quảng cáo thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(adv);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            AdvModel adv = await _context.Advs.FindAsync(Id);
            if (adv.Image != null)
            {
                var resultDelete = await _awsS3Service.DeleteFileAsync(adv.Image);
                if (resultDelete.StatusCode != 200)
                {
                    TempData["Error"] = resultDelete.Message;
                    return RedirectToAction("Index");
                }
            }
            _context.Advs.Remove(adv);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa quảng cáo thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, Status status)
        {
            var advItem = _context.Advs.FirstOrDefault(n => n.Id == id);
            if (advItem == null)
            {
                return Json(new { success = false });
            }

            advItem.Status = status;
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
                        var obj = _context.Advs.Find(Convert.ToInt32(item));
                        if (obj.Image != null)
                        {
                            await _awsS3Service.DeleteFileAsync(obj.Image);
                        }
                        _context.Advs.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
