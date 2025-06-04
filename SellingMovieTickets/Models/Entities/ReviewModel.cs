using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class ReviewModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điểm")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập bình luận")]
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        public int MovieId { get; set; }
        public MovieModel Movie { get; set; }

        public string AppUserId { get; set; }
        public AppUserModel AppUser { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
