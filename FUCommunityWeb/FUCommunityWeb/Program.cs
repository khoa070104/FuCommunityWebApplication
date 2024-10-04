using FuCommunityWebDataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FuCommunityWebModels.Models;
using FuCommunityWebUtility;
using Microsoft.AspNetCore.Identity.UI.Services;
using FuCommunityWebDataAccess.Repositories;
using FuCommunityWebServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// options => options.SignIn.RequireConfirmedAccount = true    ~ Sẽ update Confirm email sau
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//builder.Services.AddDefaultIdentity<ApplicationUser>()
//	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication().AddFacebook(option =>
{
	option.AppId = "1071287101119725";
	option.AppSecret = "b6fdf1abe67ae1a1f9ae8dbf85250f3b";
});
builder.Services.AddScoped<UserRepo>();

builder.Services.AddScoped<UserService>();

builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender,EmailSender>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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