using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FuCommunityWebServices.Services;
using FuCommunityWebModels.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FuCommunityWebModels.ViewModels.FuCommunityWebModels.ViewModels;

namespace FUCommunityWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
		private readonly HomeService _homeService;
        private readonly CourseService _courseService;

        public HomeController(ILogger<HomeController> logger, UserService userService, HomeService homeService, CourseService courseService, ApplicationDbContext context)
		{
			_logger = logger;
            _userService = userService;
            _homeService = homeService;
            _courseService = courseService;
            _context = context;
        }
        public IActionResult Index()
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sử dụng CourseService để lấy danh sách khóa học đã đăng ký
            var enrolledCourses = _courseService.GetEnrolledCoursesAsync(userId).Result;

            // Sử dụng CourseService để lấy danh sách khóa học được mua nhiều nhất
            var mostPurchasedCourses = _courseService.GetMostPurchasedCoursesAsync(3).Result;

            // Sử dụng CourseService để lấy danh sách khóa học chất lượng cao nhất
            var highestQualityCourse = _courseService.GetHighestQualityCoursesAsync(3).Result;

            var homeViewModel = new HomeVM
            {
                MostPurchasedCourses = mostPurchasedCourses,
                HighestQualityCourse = highestQualityCourse,
                EnrolledCourses = enrolledCourses
            };

            return View(homeViewModel);
        }

        public IActionResult About()
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
        
        public IActionResult ForumDetail()
        {
            return View();
        }
        public IActionResult Home()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sử dụng CourseService để lấy danh sách khóa học đã đăng ký
            var enrolledCourses = _courseService.GetEnrolledCoursesAsync(userId).Result;

            // Sử dụng CourseService để lấy danh sách khóa học được mua nhiều nhất
            var mostPurchasedCourses = _courseService.GetMostPurchasedCoursesAsync(3).Result;

            // Sử dụng CourseService để lấy danh sách khóa học chất lượng cao nhất
            var highestQualityCourse = _courseService.GetHighestQualityCoursesAsync(3).Result;

            var homeViewModel = new HomeVM
            {
                MostPurchasedCourses = mostPurchasedCourses,
                HighestQualityCourse = highestQualityCourse,
                EnrolledCourses = enrolledCourses
            };

            return View(homeViewModel);
        }

        public async Task<IActionResult> GetPosts(int page = 1, int pageSize = 2, string searchString = "")
        {
            var (posts, totalItems) = await _homeService.GetPostsAsync(page, pageSize, searchString);

            return Json(new
            {
                posts = posts,
                totalItems = totalItems,
                pageSize = pageSize,
                currentPage = page
            });
        }

        public IActionResult Post()
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