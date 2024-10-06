using FuCommunityWebModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FUCommunityWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult ManageCourse()
        {
            return View();
        }
        public IActionResult AdminDashBoard()
        {
            return View();
        }
        public IActionResult ManageForumGroup()
        {
            return View();
        }
        public IActionResult ManageForumPost()
        {
            return View();
        }
        public IActionResult ManageForumUser()
        {
            return View();
        }
        public IActionResult ManagePayment()
        {
            return View();
        }
        public IActionResult ManageStudent()
        {
            return View();
        }
        public IActionResult ManageUser()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
