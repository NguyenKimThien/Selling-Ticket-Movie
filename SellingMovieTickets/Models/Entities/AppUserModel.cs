using Microsoft.AspNetCore.Identity;
using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class AppUserModel : IdentityUser, CommonAbstract
    {
        public string? LastName { get; set; } = "";
        public string? FirstName { get; set; } = "";
        public string? FullName { get; set; } = "";
        public string? Address { get; set; } = "";
        [Required]
        public Gender Gender { get; set; }
        public string? Avatar { get; set; }
        public string? Token { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<ReviewModel> Reviews { get; set; }
    }   
}
