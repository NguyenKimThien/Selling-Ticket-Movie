using SellingMovieTickets.Models.Enum;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string Role { get; set; }
    }
}
