using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class MovieCategoryMappingModel
    {
        public int MovieId { get; set; }
        public int MovieCategoryId { get; set; }

        public MovieModel Movie { get; set; }
        public MovieCategoryModel MovieCategory { get; set; }
    }
}
