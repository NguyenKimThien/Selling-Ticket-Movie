using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.ViewModels.Users
{
    public class LoginAccount
    {
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập Password")]
        public string Password { get; set; }
    }
}
