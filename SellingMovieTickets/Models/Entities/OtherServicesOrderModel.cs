using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.Entities
{
    // bảng đơn đồ ăn/ đồ uống đi kèm khi mua vé
    public class OtherServicesOrderModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }

        public int OtherServicesId { get; set; }
        public OtherServicesModel OtherServices { get; set; }

        public int OrderId { get; set; }
        public OrderModel Order { get; set; }  

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
