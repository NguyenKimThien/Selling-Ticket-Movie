using Microsoft.AspNetCore.Mvc.Rendering;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Room
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public int RowNumber { get; set; }
        public int NumberOfSeats { get; set; }
        public int TotalSeats { get; set; }
        public string StatusRoom { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
