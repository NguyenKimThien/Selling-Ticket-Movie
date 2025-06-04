using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.Entities
{
    public class TicketModel : CommonAbstract, CommonPayment
    {
        [Key]
        public int Id { get; set; }
        public string NameMovie { get; set; }
        public string TicketCode { get; set; }
        public DateTime StartShowTime { get; set; }
        public DateTime PaymentTime { get; set; }

        [Column(TypeName = "decimal(10, 3)")]
        public decimal ConcessionAmount { get; set; } // Số tiền phụ phí bỏng nước
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }      // Tổng tiền
        [Column(TypeName = "decimal(10, 3)")]
        public decimal DiscountAmount { get; set; }   // Số tiền giảm giá
        [Column(TypeName = "decimal(10, 3)")]
        public decimal PaymentAmount { get; set; }    // Số tiền phải thanh toán

        public string SeatNames { get; set; }
        public string RoomNumber { get; set; }

        public OrderModel Order { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
