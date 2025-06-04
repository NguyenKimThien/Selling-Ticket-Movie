using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServices
{
    public class UpdateOtherServices
    {
        [Required(ErrorMessage = "Yêu cầu nhập tên dịch vụ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập giá")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(10, 3)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
        public string? Image { get; set; }
        public Status Status { get; set; }
    }
}
