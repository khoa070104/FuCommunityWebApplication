using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

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

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(IFormFile file, string currentPage)
        {
            if (file != null && file.Length > 0)
            {
                // Get the currently logged-in user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    // Define the path to save the uploaded image
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Check if the uploads directory exists, if not, create it
                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(uploadsDirectory, fileName);

                    // Save the uploaded image to the server
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Update the AvatarImage property
                    user.AvatarImage = $"/uploads/{fileName}";
                    await _context.SaveChangesAsync();
                }
            }

            // Redirect back to the current page
            return Redirect(currentPage);
        }




        public IActionResult About()
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

            var mentorViewModel = new MentorViewModel
            {
                TopMentors = topUsers,
                OtherMentors = otherUsers
            };

            return View(mentorViewModel);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.Include(u => u.IsVotes)
                .FirstOrDefault(u => u.Id == userId);


            var enrollments = _context.Enrollment
                .Where(e => e.UserID == userId)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToList();

            var model = new CourseHistoryViewModel
            {
                User = user,
                Enrollments = enrollments
            };

            ViewData["CurrentPage"] = Url.Action("CourseHistory");
            return View(model);
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
            var forumViewModel = new ForumViewModel
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
        public IActionResult ForumDetail()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult PostHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var user = _context.Users.Include(u => u.IsVotes)
                .FirstOrDefault(u => u.Id == userId);

            var posts = _context.Posts
                .Where(p => p.UserID == userId)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedDate)
                .ToList();

            var viewModel = new PostHistoryViewModel
            {
                User = user,
                Post = posts
            };

            ViewData["CurrentPage"] = Url.Action("PostHistory");
            return View(viewModel);
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
