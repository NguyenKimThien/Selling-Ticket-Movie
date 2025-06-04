using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Category;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CategoryController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<CategoryModel> categories = await _context.Categories.OrderByDescending(x => x.Id).ToListAsync();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                categories = categories.Where(x =>
                    (x.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Description?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Slug?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.SeoKeywords?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            int recsCount = categories.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = categories.Skip(resSkip).Take(pager.PageSize).ToList();
            var categoriesVM = _mapper.Map<List<CategoryViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(categoriesVM);
        }
    
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                category.Slug = GenerateSlug(category.Name);

                var slug = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    TempData["Error"] = "Danh mục đã có trong cơ sở dữ liệu";
                    return View(category);
                }
                category.CreateDate = DateTime.Now;
                category.ModifiedDate = DateTime.Now;
                category.CreateBy = nameEditor;
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(category);
            }
        }

        // Hàm tạo Slug từ tên
        private string GenerateSlug(string name)
        {
            // Chuyển thành chữ thường
            string slug = name.ToLowerInvariant();
            // Loại bỏ dấu bằng cách chuyển thành ký tự không dấu
            slug = RemoveDiacritics(slug);
            // Thay thế các khoảng trắng bằng dấu gạch ngang
            slug = slug.Replace(" ", "-");
            // Loại bỏ các ký tự không hợp lệ (không phải chữ hoặc số hoặc dấu gạch ngang)
            slug = RemoveInvalidCharacters(slug);

            return slug;
        }

        // Hàm loại bỏ các ký tự không hợp lệ
        private string RemoveInvalidCharacters(string text)
        {
            // Chỉ giữ lại các ký tự chữ cái, số và dấu gạch ngang
            return string.Concat(text.Where(c => char.IsLetterOrDigit(c) || c == '-'));
        }

        // Hàm loại bỏ dấu
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Danh mục không tồn tại";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                TempData["Error"] = "Danh mục không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.SeoKeywords = category.SeoKeywords;
                existingCategory.ModifiedBy = nameEditor;
                existingCategory.ModifiedDate = DateTime.Now;

                // Tạo slug từ tên danh mục và kiểm tra trùng lặp
                existingCategory.Slug = GenerateSlug(category.Name);

                var slug = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == existingCategory.Slug && p.Id != id);
                if (slug != null)
                {
                    TempData["Error"] = "Danh mục đã có trong cơ sở dữ liệu";
                    return View(category);
                }

                existingCategory.Status = category.Status;

                _context.Update(existingCategory);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Có lỗi trong model";
            return View(category);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _context.Categories.FindAsync(Id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa danh mục thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, Status status)
        {
            var newsItem = _context.Categories.FirstOrDefault(n => n.Id == id);
            if (newsItem == null)
            {
                return Json(new { success = false });
            }

            newsItem.Status = status;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = _context.Categories.Find(Convert.ToInt32(item));
                        _context.Categories.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

