using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Controllers
{
    public class MenuController : Controller
    {
        private readonly DataContext _context;

        public MenuController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult MenuTop()
        {
            var items = _context.Categories
                .Where(x => x.Status == Status.Show)
                .Select(x => new { x.Name, x.Slug })   
                .ToList();
            return Json(items);
        }

        public IActionResult MenuSlider()
        {
            var items = _context.Advs.Where(x => x.Status == Status.Show).ToList();
            return PartialView("_SliderPartial", items);
        }

       


    }
}
