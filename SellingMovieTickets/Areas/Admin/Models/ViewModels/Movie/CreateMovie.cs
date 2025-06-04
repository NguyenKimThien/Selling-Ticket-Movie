using Microsoft.AspNetCore.Mvc.Rendering;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie
{
    public class CreateMovie
    {
        [Required(ErrorMessage = "Yêu cầu nhập Tiêu đề")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Mô tả")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Yêu cầu chọn ngày phát hành")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập thời lượng")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn thể loại phim")]
        public IEnumerable<int> SelectedCategories { get; set; }  
        public MultiSelectList? MovieCategoryList { get; set; }


        [Required(ErrorMessage = "Yêu cầu chọn ngôn ngữ phim")]
        public string SelectedMovieLanguageFormat { get; set; }
        public SelectList? MovieLanguageFormats { get; set; } 


        [Required(ErrorMessage = "Yêu cầu chọn định dạng phim")]
        public string SelectedMovieFormat { get; set; }
        public SelectList? MovieFormats { get; set; }


        [Required(ErrorMessage = "Yêu cầu chọn trạng thái phim")]
        public string SelectedStatusMovie { get; set; }
        public SelectList? StatusMovies { get; set; }


        [Required(ErrorMessage = "Yêu cầu nhập xuất xứ ")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá vé")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(10, 3)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên đạo diễn")]
        public string Director { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên diễn viên")]
        public string Actor { get; set; }
        public string? TrailerUrl { get; set; }
        public bool IsOutstanding { get; set; }
        public Status Status { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
