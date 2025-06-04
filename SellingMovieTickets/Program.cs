using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets;
using SellingMovieTickets.Areas.Admin.Repository;
using SellingMovieTickets.Areas.Admin.Services.Implements;
using SellingMovieTickets.Areas.Admin.Services.Interfaces;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using SellingMovieTickets.Services.Implements;
using SellingMovieTickets.Services.Interfaces;
using System.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUserModel, IdentityRole>()
         .AddEntityFrameworkStores<DataContext>()
         .AddDefaultTokenProviders();

// add sql 
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// add email sender
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IAwsS3Service, AwsS3Service>();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache();

// Configuration Session Server
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.IsEssential = true;
});

// Configuration Cookie Client
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian tồn tại của cookie mặc định
    options.SlidingExpiration = true; // Gia hạn thêm thời gian của cookie nếu người dùng hoạt động trong phiên
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthorization();

builder.Services.AddTransient<IVnPayService, VNPayService>();
builder.Services.AddTransient<IAwsS3Service, AwsS3Service>();

// Configuration Login Google Account
builder.Services.AddAuthentication(options =>
{
}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseSession();
app.UseStaticFiles();

// Set culture to 'vi-VN' for dd/MM/yyyy format
var defaultDateCulture = "vi-VN";
var ci = new CultureInfo(defaultDateCulture);

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo> { ci },
    SupportedUICultures = new List<CultureInfo> { ci }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseAuthentication(); // ktra đăng nhập
app.UseAuthorization(); // phân quyền

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SeatHub>("/seat");
});

// Middleware cho việc đã login nhưng cố bấm Back quay về
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (context.User.Identity.IsAuthenticated && !context.User.IsInRole(Role.Customer))
    {
        if (path.StartsWith("/Admin/User/Login", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/Admin/HomeDashboard");
            return;
        }
    }
    await next();
});

// Middleware xử lý lỗi 404
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=HomeDashboard}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
       name: "Home",
       pattern: "trang-chu",
       defaults: new { controller = "Home", action = "Index" });

    endpoints.MapControllerRoute(
       name: "FilmSchedule",
       pattern: "lich-chieu",
       defaults: new { controller = "FilmSchedule", action = "Index" });
});

app.Run();
