using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.ViewModels.Users
{
    public class RegisterAccount
    {
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 4)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)[A-Za-z\d]{4,}$", ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ cái viết thường, 1 chữ số và tối thiểu có 4 ký tự.")]
        public string Password { get; set; }
    }
}
