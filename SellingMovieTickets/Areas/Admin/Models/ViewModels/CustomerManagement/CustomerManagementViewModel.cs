using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.CustomerManagement
{
    public class CustomerManagementViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUserModel AppUser { get; set; }
        public int TotalTicketsPurchased { get; set; } 
        public decimal TotalSpent { get; set; }
        public int CurrentPointsBalance { get; set; }  
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<OrderModel> OrderModels { get; set; }
        public ICollection<CustomerPointsHistoryModel> PointsHistory { get; set; }
    }
}
