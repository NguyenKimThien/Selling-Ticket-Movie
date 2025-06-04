using SellingMovieTickets.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.MovieCategory
{
    public class MovieCategoryViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<MovieCategoryMappingModel>? MovieCategoryMappings { get; set; }

    }
}
