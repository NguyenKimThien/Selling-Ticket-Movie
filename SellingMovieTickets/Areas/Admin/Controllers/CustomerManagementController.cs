using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.CinemaShowTime;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.CustomerManagement;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CustomerManagementController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CustomerManagementController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            var customerManagements = _context.CustomerManagements.Include(x => x.AppUser).OrderByDescending(x => x.Id).AsQueryable();

            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                customerManagements = customerManagements.Where(x =>
                    x.TotalTicketsPurchased.ToString().Equals(searchText) ||
                    x.CurrentPointsBalance.ToString().Contains(searchText));
            }

            int recsCount = customerManagements.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var pagedData = customerManagements.Skip(resSkip).Take(pager.PageSize).ToList();

            var customerManagementVM = _mapper.Map<List<CustomerManagementViewModel>>(pagedData);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(customerManagementVM);
        }

        public IActionResult Detail()
        {
            return View();
        }

    }
}
