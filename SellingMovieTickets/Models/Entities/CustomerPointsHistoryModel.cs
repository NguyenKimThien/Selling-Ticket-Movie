using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class CustomerPointsHistoryModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public CustomerManagementModel Customer { get; set; }

        [Required]
        public int? OrderId { get; set; }  
        public OrderModel Order { get; set; }  

        public int PointsChanged { get; set; }
        public DateTime TransactionDate { get; set; }
        public PointChangeStatus PointChangeStatus { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
