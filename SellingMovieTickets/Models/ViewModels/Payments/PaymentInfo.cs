using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.ViewModels.Payments
{
    public class PaymentInfo
    {
        public int ShowTimeId { get; set; }
        public int? PromotionId { get; set; }
        public string PaymentType { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal ConcessionAmount { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal DiscountAmount { get; set; }    
        [Column(TypeName = "decimal(10, 3)")]
        public decimal PaymentAmount { get; set; }   

        public List<InforSeat> OrderSeats { get; set; }
        public List<InforOtherServices> OtherServices { get; set; }
    }

    public class InforSeat
    {
        public int Id { get; set; }
        public string SeatName { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal Price { get; set; }
    }

    public class InforOtherServices
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public double TotalAmount { get; set; }
    }
}
