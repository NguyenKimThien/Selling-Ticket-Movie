using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Order;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Ticket;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TicketController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TicketController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<TicketModel> tickets = await _context.Tickets.OrderByDescending(x => x.Id).ToListAsync();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                tickets = tickets.Where(x =>
                  (x.NameMovie?.Contains(searchText) ?? false) ||
                  (x.TicketCode?.Contains(searchText) ?? false) ||
                  (x.SeatNames?.Contains(searchText) ?? false) ||
                  (x.RoomNumber?.Contains(searchText) ?? false) ||
                  (x.PaymentAmount == decimal.Parse(searchText)));
            }

            int recsCount = tickets.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = tickets.Skip(resSkip).Take(pager.PageSize).ToList();
            var ticketsVM = _mapper.Map<List<TicketViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(ticketsVM);
        }
    }
}
