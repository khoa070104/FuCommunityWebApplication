using FuCommunityWebDataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FuCommunityWebModels.Models;
using FuCommunityWebUtility;
using Microsoft.AspNetCore.Identity.UI.Services;
using FuCommunityWebDataAccess.Repositories;
using FuCommunityWebServices.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// Đăng ký các dịch vụ cho các controller
builder.Services.AddScoped<UserRepo>(); 
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<HomeRepo>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<CourseRepo>(); // Đăng ký CourseRepo
builder.Services.AddScoped<CourseService>(); // Đăng ký CourseService

//var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
//var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

//builder.Services.AddAuthentication().AddFacebook(option =>
//{
//	option.AppId = facebookAppId;
//	option.AppSecret = facebookAppSecret;
//});
builder.Services.AddAuthentication().AddGoogle(option =>
{
	option.ClientId = googleClientId;
	option.ClientSecret = googleClientSecret;
});
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender,EmailSender>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
