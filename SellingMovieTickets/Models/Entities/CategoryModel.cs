using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class CategoryModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên Danh mục")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mô tả Danh mục")]
        public string Description { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeywords { get; set; }
        public Status Status { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
