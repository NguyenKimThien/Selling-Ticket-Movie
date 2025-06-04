using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Seat
{
    public class SeatViewModel
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsHeld { get; set; }
        public DateTime HoldUntil { get; set; }
        public string HeldByUserId { get; set; }
        public int CinemaShowTimeId { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
