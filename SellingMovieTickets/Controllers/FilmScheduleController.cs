using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Room;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Seat;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.CinemaShowTimes;
using SellingMovieTickets.Repository;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Controllers
{
    public class FilmScheduleController : Controller
    {
        private DataContext _context;
        private readonly IHubContext<SeatHub> _hubContext;

        public FilmScheduleController(DataContext context, IHubContext<SeatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetShowTimes")]
        public async Task<IActionResult> GetShowTimes()
        {
            var today = DateTime.Today;
            var fiveDaysLater = today.AddDays(5);

            var showTimes = await _context.CinemaShowTimes
                .Where(st => st.StartShowTime >= today && st.StartShowTime < fiveDaysLater)
                .Select(st => new
                {
                    st.Id,
                    st.StartShowTime,
                    st.EndShowTime,
                    Movie = st.Movie,
                    Genres = string.Join(", ", st.Movie.MovieCategoryMappings.Select(mcm => mcm.MovieCategory.CategoryName))

                })
                .ToListAsync();

            var result = showTimes.Select(st => new
            {
                st.Id,
                st.StartShowTime,
                st.EndShowTime,
                Movie = MapToMovieViewModel(st.Movie, st.Genres)
            }).ToList();

            return new JsonResult(result);
        }

        private static MovieViewModel MapToMovieViewModel(MovieModel movie, string Genres)
        {
            return new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Duration = movie.Duration,
                MovieLanguageFormat = movie.MovieLanguageFormat,
                MovieFormat = movie.MovieFormat,
                Rating = movie.Rating,
                StatusMovie = movie.StatusMovie,
                Origin = movie.Origin,
                Price = movie.Price,
                Director = movie.Director,
                Actor = movie.Actor,
                TrailerUrl = movie.TrailerUrl,
                Status = movie.Status,
                IsOutstanding = movie.IsOutstanding,
                Image = movie.Image,
                CreateBy = movie.CreateBy,
                CreateDate = movie.CreateDate,
                Genres = Genres
            };
        }

        private static RoomViewModel MapToRoomViewModel(RoomModel room)
        {
            return new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RowNumber = room.RowNumber,
                NumberOfSeats = room.NumberOfSeats,
                TotalSeats = room.TotalSeats
            };
        }

        private static List<SeatViewModel> MapToSeatViewModel(List<SeatModel> seats)
        {
            return seats.Select(seat => new SeatViewModel
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                IsAvailable = seat.IsAvailable,
                IsHeld = seat.IsHeld,
                HoldUntil = seat.HoldUntil,
                HeldByUserId = seat.HeldByUserId
            }).ToList();
        }

        public async Task<IActionResult> Detail(int id, string time)
        {
            var showTimes = await _context.CinemaShowTimes
                .Include(x => x.Room)
                .Include(x => x.Movie)
                .ThenInclude(m => m.MovieCategoryMappings)
                .ThenInclude(mcm => mcm.MovieCategory)
                .Where(st => st.Id == id)
                .Select(st => new
                {
                    st.StartShowTime,
                    st.Room,
                    Movie = st.Movie,
                    Genres = string.Join(", ", st.Movie.MovieCategoryMappings.Select(mcm => mcm.MovieCategory.CategoryName))
                }).FirstOrDefaultAsync();

            if (showTimes == null)
            {
                return NotFound();
            }
            var seats = await _context.Seats.Where(seat => seat.CinemaShowTimeId == id).ToListAsync();

            var movieST = new CinemaShowTimeVM
            {
                Id = id,
                StartShowTime = showTimes.StartShowTime,
                MovieVM = MapToMovieViewModel(showTimes.Movie, showTimes.Genres),
                RoomVM = MapToRoomViewModel(showTimes.Room),
                Seats = MapToSeatViewModel(seats)
            };

            return View(movieST);
        }
    }
}
