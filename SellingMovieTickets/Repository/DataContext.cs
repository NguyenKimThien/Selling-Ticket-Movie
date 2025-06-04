using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SellingMovieTickets.Models.Entities;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SellingMovieTickets.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<MovieModel> Movies { get; set; }
        public DbSet<CinemaShowTimeModel> CinemaShowTimes { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<SeatModel> Seats { get; set; }
        public DbSet<AppUserModel> Users { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }
        public DbSet<PromotionModel> Promotions { get; set; }
        public DbSet<OtherServicesModel> OtherServices { get; set; }
        public DbSet<OtherServicesOrderModel> OtherServicesOrders { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<MovieCategoryModel> MovieCategories { get; set; }
        public DbSet<MovieCategoryMappingModel> MovieCategoryMappings { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<AdvModel> Advs { get; set; }
        public DbSet<NewsModel> News { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<CustomerManagementModel> CustomerManagements { get; set; }
        public DbSet<CustomerPointsModel> CustomerPoints { get; set; }
        public DbSet<CustomerPointsHistoryModel> CustomerPointsHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerManagementModel>()
                .HasOne(c => c.AppUser)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập khóa chính cho bảng MovieCategoryMapping
            modelBuilder.Entity<MovieCategoryMappingModel>()
                .HasKey(mc => new { mc.MovieId, mc.MovieCategoryId });

            // Thiết lập quan hệ nhiều-nhiều giữa Movie và MovieCategory
            modelBuilder.Entity<MovieCategoryMappingModel>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCategoryMappings)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCategoryMappingModel>()
                .HasOne(mc => mc.MovieCategory)
                .WithMany(c => c.MovieCategoryMappings)
                .HasForeignKey(mc => mc.MovieCategoryId);

            // Giữ cascade delete giữa CinemaShowTime và List<SeatModel>
            modelBuilder.Entity<SeatModel>()
                .HasOne(s => s.CinemaShowTime)
                .WithMany(st => st.Seats)
                .HasForeignKey(s => s.CinemaShowTimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Đổi thành DeleteBehavior.Restrict cho khóa ngoại từ CinemaShowTimeModel đến RoomModel
            modelBuilder.Entity<CinemaShowTimeModel>()
                .HasOne(c => c.Room)
                .WithMany(r => r.ShowTimes)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ OrderModel -> OrderDetailModel
            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ OrderModel -> OtherServicesOrderModel
            modelBuilder.Entity<OtherServicesOrderModel>()
                .HasOne(os => os.Order)
                .WithMany(o => o.OtherServicesOrders)
                .HasForeignKey(os => os.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ OrderModel -> TicketModel
            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.Ticket)
                .WithOne(t => t.Order)
                .HasForeignKey<OrderModel>(o => o.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập mối quan hệ 1-1: Order và CustomerPointHistory
            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.CustomerPointsHistory)
                .WithOne(c => c.Order)
                .HasForeignKey<CustomerPointsHistoryModel>(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerManagementModel>()
                .HasOne(cm => cm.AppUser)
                .WithMany()
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
