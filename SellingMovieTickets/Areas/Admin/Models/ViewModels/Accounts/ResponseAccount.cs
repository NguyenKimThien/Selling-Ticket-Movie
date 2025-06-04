using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Accounts
{
    public class ResponseAccount
    {
        public AppUserModel User { get; set; }
        public string Role { get; set; }
    }

}
