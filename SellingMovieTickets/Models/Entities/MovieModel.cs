using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.Entities
{
    public class MovieModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Tiêu đề")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Mô tả")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn ngày phát hành")]
        public DateTime ReleaseDate { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập thời lượng")]
        public int Duration { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn ngôn ngữ")]
        public string MovieLanguageFormat { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn định dạng phim")]
        public string MovieFormat { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập giá")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(10, 3)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên đạo diễn")]
        public string Director { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên diễn viên")]
        public string Actor { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn tình trạng phim")]
        public string StatusMovie { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập xuất xứ")]
        public string Origin { get; set; }
        public bool IsOutstanding { get; set; }
        public string? Rating { get; set; }
        public string? TrailerUrl { get; set; }
        public Status Status { get; set; }
        public string? Image { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<CinemaShowTimeModel>? CinemaShowTimes { get; set; }
        public ICollection<MovieCategoryMappingModel>? MovieCategoryMappings { get; set; }
    }
}
