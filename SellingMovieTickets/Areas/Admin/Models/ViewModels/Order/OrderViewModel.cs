using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public string PaymentType { get; set; }
        public int NumberOfTickets { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }
        public string TicketCode { get; set; }
        public string StatusOrder { get; set; }

        public int TicketId { get; set; }
        public int CinemaShowTimeId { get; set; }
        public int CustomerManagementId { get; set; }
        public int? PromotionId { get; set; }
 
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
