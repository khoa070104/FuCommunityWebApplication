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
using Microsoft.Extensions.Hosting;

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
        public async Task<IActionResult> Forum()
        {
            var courses = await _homeService.GetAllCoursesAsync();
            var category = await _homeService.GetAllCategoryAsync();

            var forumViewModel = new ForumVM
            {
                Courses = courses,
                Categories = category
            };

            return View(forumViewModel);
        }

        public IActionResult PostDetail()
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

        [HttpGet]
        public async Task<IActionResult> GetPosts(int categoryID, int page = 1, int pageSize = 2, string searchString = "")
        {
            var (posts, totalItems) = await _homeService.GetPostsByCategory(categoryID, page, pageSize, searchString);

            return Json(new
            {
                posts = posts,
                totalItems = totalItems,
                pageSize = pageSize,
                currentPage = page
            });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostVM postVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (postVM.CreatePostVM.PostImageFile != null && postVM.CreatePostVM.PostImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(postVM.CreatePostVM.PostImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postVM.CreatePostVM.PostImageFile.CopyToAsync(fileStream);
                }
                postVM.CreatePostVM.PostImage = "/uploads/" + uniqueFileName;
            }
            else
            {
                postVM.CreatePostVM.PostImage = "/uploads/default-image.png";
            }

            var post = new Post
            {
                Title = postVM.CreatePostVM.Title,
                Content = postVM.CreatePostVM.Content,
                CategoryID = postVM.CreatePostVM.CategoryID,
                UserID = userId,
                CreatedDate = DateTime.Now,
                Status = "Published",
                Tag = postVM.CreatePostVM.Tag,
                PostImage = postVM.CreatePostVM.PostImage
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Forum");
        }

        public IActionResult Post(CategoryVM categoryVM)
        {
            var modal = new PostVM
            {
                CategoryVM = new CategoryVM
                {
                    CategoryName = categoryVM.CategoryName,
                    CategoryID = categoryVM.CategoryID
                }
            };
            return View(modal);
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