using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class CustomerPointsModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public CustomerManagementModel Customer { get; set; }

        public int PointsEarned { get; set; } // Tổng điểm kiếm được
        public int PointsRedeemed { get; set; } // Tổng điểm đã sử dụng

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

}
