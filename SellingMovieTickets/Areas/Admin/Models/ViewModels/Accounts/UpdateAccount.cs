using SellingMovieTickets.Areas.Admin.Models.ViewModels.User;
using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Accounts
{
    public class UpdateAccount
    {
        public UpdateUser User { get; set; }
        public string Role { get; set; }
    }
}
