using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace SellingMovieTickets.Models.Entities
{
    public class CinemaShowTimeModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn thời gian chiếu phim")]
        public DateTime StartShowTime { get; set; }
        public DateTime EndShowTime { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn phòng chiếu")]
        public int RoomId { get; set; }
        public RoomModel Room { get; set; }

        public int MovieId { get; set; }
        public MovieModel Movie { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<SeatModel> Seats { get; set; }   
    }

}
