using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FUCommunityWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to Blazor dashboard page
            return Redirect("/dashboard");
        }

        // Action để render Blazor component trong MVC view
        public IActionResult BlazorDashboard()
        {
            // Lấy userId từ claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            // Debug log
            System.Diagnostics.Debug.WriteLine($"DEBUG DashboardController: User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"DEBUG DashboardController: UserId: '{userId}'");
            Console.WriteLine($"CONSOLE DashboardController: User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"CONSOLE DashboardController: UserId: '{userId}'");

            ViewBag.UserId = userId;
            return View();
        }

        // Test action để kiểm tra authentication
        public IActionResult Test()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            Console.WriteLine($"CONSOLE DashboardController.Test: User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"CONSOLE DashboardController.Test: UserId: '{userId}'");
            return Json(new { IsAuthenticated = User.Identity?.IsAuthenticated, UserId = userId });
        }
    }
}
