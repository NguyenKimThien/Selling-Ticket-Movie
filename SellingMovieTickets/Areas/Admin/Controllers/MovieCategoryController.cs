using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.MovieCategory;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.CinemaShowTimes;
using SellingMovieTickets.Repository;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class MovieCategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MovieCategoryController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<MovieCategoryModel> categorieMovies = await _context.MovieCategories.OrderByDescending(x => x.Id).ToListAsync();
            const int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                categorieMovies = categorieMovies.Where(x =>
                    (x.CategoryName?.Contains(searchText) ?? false));
            }

            int recsCount = categorieMovies.Count();

            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = categorieMovies.Skip(resSkip).Take(pager.PageSize).ToList();
            var movieCategoriesVM = _mapper.Map<List<MovieCategoryViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(movieCategoriesVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCategoryModel movieCategory)
        {
            if (ModelState.IsValid)
            {
                var existingMovieCategory = await _context.MovieCategories.FirstOrDefaultAsync(p => p.CategoryName == movieCategory.CategoryName);

                if (existingMovieCategory != null)
                {
                    TempData["Error"] = "Thể loại phim đã có trong cơ sở dữ liệu";
                    return View(movieCategory);
                }

                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                movieCategory.CreateDate = DateTime.Now;
                movieCategory.ModifiedDate = DateTime.Now;
                movieCategory.CreateBy = nameEditor;
                _context.Add(movieCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thể loại phim thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(movieCategory);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var movieCategory = await _context.MovieCategories.FindAsync(id);
            if (movieCategory == null)
            {
                TempData["Error"] = "Thể loại phim không tồn tại";
                return RedirectToAction("Index");
            }
            return View(movieCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieCategoryModel movieCategory)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var existingMovieCategory = await _context.MovieCategories.FindAsync(id);
            if (existingMovieCategory == null)
            {
                TempData["Error"] = "Thể loại phim không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingMovieCategory.CategoryName = movieCategory.CategoryName;
                existingMovieCategory.ModifiedBy = nameEditor;
                existingMovieCategory.ModifiedDate = DateTime.Now;

                _context.Update(existingMovieCategory);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật thể loại phim thành công";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Có lỗi trong model";
            return View(movieCategory);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            MovieCategoryModel movieCategory = await _context.MovieCategories.FindAsync(Id);
            _context.MovieCategories.Remove(movieCategory);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa thể loại phim thành công";
            return RedirectToAction("Index");
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
                        var obj = _context.MovieCategories.Find(Convert.ToInt32(item));
                        _context.MovieCategories.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
