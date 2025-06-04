using SellingMovieTickets.Areas.Admin.Models.ViewModels.User;
using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Accounts
{
    public class CreateAccount
    {
        public RegisterUser User { get; set; }
        public string Role { get; set; }
    }
}
