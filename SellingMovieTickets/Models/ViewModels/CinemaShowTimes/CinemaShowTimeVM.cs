using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Room;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Seat;
using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.ViewModels.CinemaShowTimes
{
    public class CinemaShowTimeVM
    {
        public int Id { get; set; }
        public DateTime StartShowTime { get; set; }
        public DateTime EndShowTime { get; set; }
        public RoomViewModel RoomVM { get; set; }
        public MovieViewModel MovieVM { get; set; }
        public List<SeatViewModel> Seats { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
