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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyCourse(int courseId, string returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }

            var alreadyEnrolled = await _context.Enrollment
                .AnyAsync(e => e.UserID == userId && e.CourseID == courseId);

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Kiểm tra xem người dùng có đủ điểm để mua khóa học không
            if (user.Point < course.Price)
            {
                TempData["Error"] = "You do not have enough points to purchase this course.";
                return RedirectToAction("Index");
            }

            // Trừ điểm của người dùng
            user.Point -= course.Price.Value;

            var enrollment = new Enrollment
            {
                UserID = userId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            // Thêm đăng ký khóa học
            _context.Enrollment.Add(enrollment);
            await _context.SaveChangesAsync();

            // Cập nhật điểm của người dùng
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment successful!";

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
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