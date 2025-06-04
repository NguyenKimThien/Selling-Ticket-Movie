using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Ticket
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string NameMovie { get; set; }
        public string TicketCode { get; set; }
        public DateTime StartShowTime { get; set; }
        public DateTime PaymentTime { get; set; }


        [Column(TypeName = "decimal(10, 3)")]
        public decimal ConcessionAmount { get; set; }  
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }     
        [Column(TypeName = "decimal(10, 3)")]
        public decimal DiscountAmount { get; set; }    
        [Column(TypeName = "decimal(10, 3)")]
        public decimal PaymentAmount { get; set; }    

        public string SeatNames { get; set; }
        public string RoomNumber { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
