using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.ViewModels.Accounts
{
    public class ResetPassword
    {
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập Password")]
        public string Password { get; set; }
    }
}
