using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class MovieCategoryModel: CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Tên thể loại")]
        public string CategoryName { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<MovieCategoryMappingModel>? MovieCategoryMappings { get; set; }
    }
}
