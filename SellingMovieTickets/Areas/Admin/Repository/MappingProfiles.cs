using AutoMapper;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Adv;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Category;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.CinemaShowTime;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.CustomerManagement;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.MovieCategory;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.News;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Order;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OrderDetail;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServices;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.OtherServicesOrder;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Room;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Ticket;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.User;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.ViewModels.CinemaShowTimes;
using System.Security;

namespace SellingMovieTickets.Areas.Admin.Repository
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AdvModel, AdvViewModel>().ReverseMap();
            CreateMap<CategoryModel, CategoryViewModel>().ReverseMap();
            CreateMap<CinemaShowTimeModel, CinemaShowTimeViewModel>()
                      .ForMember(dest => dest.NumberRoom, opt => opt.MapFrom(src => src.Room.RoomNumber))
                      .ForMember(dest => dest.NameMovie, opt => opt.MapFrom(src => src.Movie.Name)); 
            CreateMap<MovieModel, MovieViewModel>()
                      .ForMember(dest => dest.Genres, opt =>
                          opt.MapFrom(src => string.Join(", ", src.MovieCategoryMappings.Select(mcm => mcm.MovieCategory.CategoryName)))).ReverseMap();
            CreateMap<MovieCategoryModel, MovieCategoryViewModel>().ReverseMap();
            CreateMap<NewsModel, NewsViewModel>().ReverseMap();
            CreateMap<OtherServicesModel, OtherServicesViewModel>().ReverseMap();
            CreateMap<RoomModel, RoomViewModel>().ReverseMap();
            CreateMap<AppUserModel, UserViewModel>().ReverseMap();
            CreateMap<OrderModel, OrderViewModel>().ReverseMap();
            CreateMap<TicketModel, TicketViewModel>().ReverseMap();
            CreateMap<OrderDetailModel, OrderDetailViewModel>().ReverseMap();
            CreateMap<OtherServicesOrderModel, OtherServicesOrderViewModel>().ReverseMap();
            CreateMap<CustomerManagementModel, CustomerManagementViewModel>()
                    .ForPath(dest => dest.AppUser.FullName, opt => opt.MapFrom(src => src.AppUser.FullName));

        }
    }
}
