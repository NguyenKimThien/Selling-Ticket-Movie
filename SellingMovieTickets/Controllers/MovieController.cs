using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Controllers
{
    public class MovieController : Controller
    {
        private readonly DataContext _context;

        public MovieController(DataContext context)
        {
            _context = context;
        }

        public IActionResult MovieNowShowing()
        {
            var moviesQuery = _context.Movies.Include(m => m.MovieCategoryMappings)
            .ThenInclude(mcm => mcm.MovieCategory)
            .Where(x => x.Status == Status.Show && x.IsOutstanding && x.StatusMovie == StatusMovie.NowShowing) 
            .OrderByDescending(x => x.Id)
            .Take(8)  
            .AsQueryable();

            var movieViewModel = moviesQuery.Select(x => new MovieViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ReleaseDate = x.ReleaseDate,
                Duration = x.Duration,
                MovieLanguageFormat = x.MovieLanguageFormat,
                MovieFormat = x.MovieFormat,
                StatusMovie = x.StatusMovie,
                Origin = x.Origin,
                Price = x.Price,
                Director = x.Director,
                Actor = x.Actor,
                Rating = x.Rating,
                TrailerUrl = x.TrailerUrl,
                Status = x.Status,
                Image = x.Image,
                CreateBy = x.CreateBy,
                CreateDate = x.CreateDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                Genres = string.Join(", ", x.MovieCategoryMappings.Select(mcm => mcm.MovieCategory.CategoryName))
            }).ToList();

            return PartialView("_MovieNowShowingPartial", movieViewModel);
        }

        public IActionResult MovieCommingSoon()
        {
            var moviesQuery = _context.Movies.Include(m => m.MovieCategoryMappings)
            .ThenInclude(mcm => mcm.MovieCategory)
            .Where(x => x.Status == Status.Show && x.IsOutstanding && x.StatusMovie == StatusMovie.CommingSoon)
            .OrderByDescending(x => x.Id)
            .Take(8)
            .AsQueryable();

            var movieViewModel = moviesQuery.Select(x => new MovieViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ReleaseDate = x.ReleaseDate,
                Duration = x.Duration,
                MovieLanguageFormat = x.MovieLanguageFormat,
                MovieFormat = x.MovieFormat,
                StatusMovie = x.StatusMovie,
                Origin = x.Origin,
                Price = x.Price,
                Director = x.Director,
                Actor = x.Actor,
                Rating = x.Rating,
                TrailerUrl = x.TrailerUrl,
                Status = x.Status,
                Image = x.Image,
                CreateBy = x.CreateBy,
                CreateDate = x.CreateDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                Genres = string.Join(", ", x.MovieCategoryMappings.Select(mcm => mcm.MovieCategory.CategoryName))
            }).ToList();

            return PartialView("_MovieCommingSoonPartial", movieViewModel);
        }



    }
}
