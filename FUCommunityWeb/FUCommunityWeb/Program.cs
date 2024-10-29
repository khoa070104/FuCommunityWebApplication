using FuCommunityWebDataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FuCommunityWebModels.Models;
using FuCommunityWebUtility;
using Microsoft.AspNetCore.Identity.UI.Services;
using FuCommunityWebDataAccess.Repositories;
using FuCommunityWebServices.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity for IdentityUser
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure IdentityCore for ApplicationUser
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register ApplicationUserService
builder.Services.AddScoped<ApplicationUserService>();

// Register repositories and services for dependency injection
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<HomeRepo>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<CourseRepo>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<ForumRepo>();
builder.Services.AddScoped<ForumService>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AdminRepo>();

// Register VnPayService for VNPAY integration
builder.Services.AddScoped<VnPayService>();

// Configure External Authentication (Google)
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

builder.Services.AddAuthentication()
    .AddGoogle(option =>
    {
        option.ClientId = googleClientId;
        option.ClientSecret = googleClientSecret;
    });

// Register Razor Pages
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.SlidingExpiration = true;
});

// Register Email Sender Service
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Thêm vào phần ConfigureServices
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userService = services.GetRequiredService<ApplicationUserService>();
    await userService.SeedAdminUser();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Thêm vào phần Configure, trước app.UseAuthentication()
app.UseSession();

// Map Razor Pages
app.MapRazorPages();

// Update the default controller route to VnPayController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
