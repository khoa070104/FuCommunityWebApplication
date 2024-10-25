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
using System.Net;

namespace FUCommunityWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly HomeService _homeService;
        private readonly CourseService _courseService;
        private readonly ForumService _forumService;

        public HomeController(ILogger<HomeController> logger, UserService userService, HomeService homeService, CourseService courseService, ApplicationDbContext context, ForumService forumService)
        {
            _logger = logger;
            _userService = userService;
            _homeService = homeService;
            _courseService = courseService;
            _forumService = forumService;
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var enrolledCourses = _courseService.GetEnrolledCoursesAsync(userId).Result;
            var mostPurchasedCourses = _courseService.GetMostPurchasedCoursesAsync(3).Result;
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

        public IActionResult Home()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var enrolledCourses = _courseService.GetEnrolledCoursesAsync(userId).Result;

            var mostPurchasedCourses = _courseService.GetMostPurchasedCoursesAsync(3).Result;

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

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            if (user.Point < course.Price)
            {
                TempData["Error"] = "You do not have enough points to purchase this course.";
                return RedirectToAction("Index");
            }

            user.Point -= course.Price.Value;

            var enrollment = new Enrollment
            {
                UserID = userId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            _context.Enrollment.Add(enrollment);
            await _context.SaveChangesAsync();

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
        public IActionResult ViewUser(string searchTerm = "")
        {
            var users = _userService.GetAllUsersAsync().Result;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewData["searchTerm"] = searchTerm;
            return View(users);
        }
        public IActionResult ViewUserProfile(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("ViewUser");
            }

            var user = _userService.GetUserByIdAsync(userId).Result;

            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var followers = _context.Followers
                .Where(f => f.FollowId == userId)
                .Include(f => f.FollowingUser)
                .Select(f => f.FollowingUser)
                .ToList();

            var isMentor = _userService.IsUserInRoleAsync(userId, "mentor").Result;

            var userViewModel = new UserVM
            {
                User = user,
                Enrollments = _context.Enrollment
                    .Where(e => e.UserID == userId)
                    .Include(e => e.Course)
                    .ToList(),
                Post = _context.Posts.Where(p => p.UserID == userId).ToList(),
                IsCurrentUser = (userId == currentUserId),
                IsFollowing = _userService.IsFollowingAsync(currentUserId, userId).Result,
                Followers = followers,
                IsMentor = isMentor,

                TotalPosts = _context.Posts.Count(p => p.UserID == userId && p.Type == 1),
                TotalQuestions = _context.Posts.Count(p => p.UserID == userId && p.Type == 2)
            };

            return View(userViewModel);
        }

        public async Task<IActionResult> ToggleFollow(string followId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(followId))
            {
                return BadRequest("Invalid user or follow ID.");
            }

            if (await _userService.IsFollowingAsync(userId, followId))
            {
                await _userService.UnfollowUserAsync(userId, followId);
            }
            else
            {
                await _userService.FollowUserAsync(userId, followId);
            }
            return RedirectToAction("ViewUserProfile", new { userId = followId });
        }
    }
}
