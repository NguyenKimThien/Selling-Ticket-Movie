using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class RoomModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string RoomNumber { get; set; }
        public int TotalSeats { get; set; }
        public int RowNumber { get; set; }
        public int NumberOfSeats { get; set; }
        public string StatusRoom { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<CinemaShowTimeModel> ShowTimes { get; set; } 
    }
}
