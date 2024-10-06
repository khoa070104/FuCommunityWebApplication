
using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FUCommunityWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
            _context = context;
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

        public IActionResult MentorHall()
        {
            var users = _context.Users
                .Include(u => u.IsVotes)
                .ToList();

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

        public IActionResult Forum()
        {
            var forumViewModel = new ForumVM
            {
                Courses = _context.Courses.ToList(),
                Posts = _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Votes)
            .OrderByDescending(p => p.CreatedDate)
                .ToList()
            };


            return View(forumViewModel);
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
