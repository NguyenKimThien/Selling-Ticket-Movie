using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServices;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Controllers
{
    public class OtherServicesController : Controller
    {

        private readonly DataContext _dataContext;

        public OtherServicesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetListOtherServices()
        {
            var list = await _dataContext.OtherServices
                 .Select(service => new OtherServicesViewModel
                 {
                     Id = service.Id,
                     Name = service.Name,
                     Price = service.Price,
                     Description = service.Description,
                     Image = service.Image,
                     Status = service.Status,
                     CreateBy = service.CreateBy,
                     CreateDate = service.CreateDate,
                     ModifiedBy = service.ModifiedBy,
                     ModifiedDate = service.ModifiedDate
                 })
                 .ToListAsync();
            return Json(list);
        }


    }
}
