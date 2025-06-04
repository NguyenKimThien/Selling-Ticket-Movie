using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.OrderDetail
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public OrderModel Order { get; set; }
        public string OrderCode { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
