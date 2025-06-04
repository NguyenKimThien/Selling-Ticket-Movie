using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.ViewModels.AccountManagement
{
    public class HistoryTicket
    {
        public DateTime TradingDate { get; set; }
        public string NameMovie { get; set; }
        public int NumberOfTickets { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }
    }
}
