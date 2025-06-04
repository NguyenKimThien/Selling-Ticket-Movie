using SellingMovieTickets.Areas.Admin.Models.ViewModels.OrderDetail;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServicesOrder;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.OrderDetailWithServices
{
    public class OrderDetailWithServices
    {
        public List<OrderDetailViewModel> orderDetailVM { get; set; }
        public List<OtherServicesOrderViewModel> otherServicesOrderVM { get; set; }
    }
}
