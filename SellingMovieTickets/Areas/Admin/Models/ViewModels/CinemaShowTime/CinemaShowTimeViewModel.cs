using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.CinemaShowTime
{
    public class CinemaShowTimeViewModel
    {
        public int Id { get; set; }
        public DateTime StartShowTime { get; set; }
        public DateTime EndShowTime { get; set; }

        public string NumberRoom { get; set; }
        public string NameMovie { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
