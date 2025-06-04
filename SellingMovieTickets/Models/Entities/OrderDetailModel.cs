using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.Entities
{
    public class OrderDetailModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public OrderModel Order { get; set; }
        public string SeatNumber { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal Price { get; set; } 

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

}
