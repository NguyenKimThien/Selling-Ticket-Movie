using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.CinemaShowTime
{
    public class UpdateCinemaShowTime
    {
        [Required(ErrorMessage = "Yêu cầu chọn Thời gian chiếu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime StartShowTime { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn Phim chiếu")]
        public int SelectedMovieId { get; set; }
        public SelectList? MovieIds { get; set; }
    }
}
