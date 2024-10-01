using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FUCommunityWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}
        public IActionResult Index()
		{
			return View();
		}

        public IActionResult About()
        {
            return View();
        }

        public IActionResult MentorHall()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Course()
        {
            return View();
        }
        public IActionResult CourseDetail()
        {
            return View();
        }
        public IActionResult CourseHistory()
        {
            return View();
        }
        public IActionResult Deposit()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        public IActionResult Forum()
        {
            return View();
        }
        public IActionResult ForumDetail()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
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
