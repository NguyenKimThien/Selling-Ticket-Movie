using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Room
{
    public class UpdateRoom
    {
        [Required(ErrorMessage = "Yêu cầu nhập số phòng")]
        public string RoomNumber { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số hàng")]
        public int RowNumber { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số ghế 1 hàng")]
        public int NumberOfSeats { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn trạng thái phòng")]
        public string SelectedStatusRoom { get; set; }
        public SelectList? StatusRooms { get; set; }
    }
}
