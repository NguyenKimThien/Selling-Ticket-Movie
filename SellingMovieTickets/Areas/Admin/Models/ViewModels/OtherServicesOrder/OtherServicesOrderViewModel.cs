using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServicesOrder
{
    public class OtherServicesOrderViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }
        public OtherServicesModel OtherServices { get; set; }
 
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
