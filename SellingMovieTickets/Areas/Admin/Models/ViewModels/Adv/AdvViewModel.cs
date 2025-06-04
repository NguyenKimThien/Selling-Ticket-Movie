using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Adv
{
    public class AdvViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string? Image { get; set; }

        public string? Link { get; set; }

        public Status Status { get; set; }

        public string? CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

}
