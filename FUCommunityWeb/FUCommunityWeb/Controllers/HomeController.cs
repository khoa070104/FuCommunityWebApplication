using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FuCommunityWebServices.Services;

namespace FUCommunityWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserService _userService;
		private readonly HomeService _homeService;

        public HomeController(ILogger<HomeController> logger, UserService userService, HomeService homeService)
		{
			_logger = logger;
            _userService = userService;
            _homeService = homeService;
		}
        public IActionResult Index()
		{
			return View();
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
        
        public IActionResult ForumDetail()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }

        public async Task<IActionResult> MentorHall()
        {
            var users = await _homeService.GetAllUsersWithVotesAsync();

            var sortedUsers = users
                .OrderByDescending(u => u.Point)
                .ToList();

            var topUsers = sortedUsers.Take(3).ToList();
            var otherUsers = sortedUsers.Skip(3).ToList();

            var mentorViewModel = new MentorVM
            {
                TopMentors = topUsers,
                OtherMentors = otherUsers
            };

            return View(mentorViewModel);
        }

        public async Task<IActionResult> Forum()
        {
            var courses = await _homeService.GetAllCoursesAsync();
            var posts = await _homeService.GetAllPostsAsync();

            var forumViewModel = new ForumVM
            {
                Courses = courses,
                Posts = posts
            };

            return View(forumViewModel);
        }

        [HttpGet]
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
