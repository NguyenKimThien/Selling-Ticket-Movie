using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.User
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 4)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)[A-Za-z\d]{4,}$", ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ cái viết thường, 1 chữ số và tối thiểu có 4 ký tự.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; } = "";
        [Required]
        public Gender Gender { get; set; }
        public string? Avatar { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
