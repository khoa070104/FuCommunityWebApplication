using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuCommunityWebServices.Services;
using System.Threading.Tasks;
using FuCommunityWebModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using FuCommunityWebDataAccess.Data;

namespace FUCommunityWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserService _userService;
        private UserVM userVM;

		public ProfileController(UserManager<IdentityUser> userManager, UserService userService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);
            userVM = new()
            {
				User = user,
            };
            ViewData["CurrentPage"] = Url.Action("Index");
            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(IFormFile file, string currentPage)
        {
            if (file != null && file.Length > 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var fileName = Path.GetFileName(file.FileName);
                var encryptedFileName = "avt_" + EncryptFileName(fileName);
                var path = Path.Combine(uploadsDirectory, encryptedFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var avatarPath = $"/uploads/{encryptedFileName}";
                await _userService.UpdateUserAvatarAsync(userId, avatarPath);
            }

            return Redirect(currentPage);
        }

        private string EncryptFileName(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            string fileNameWithAvatar = "avatar_" + fileNameWithoutExtension;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(fileNameWithAvatar);
            var encryptedFileName = System.Convert.ToBase64String(plainTextBytes);

            return encryptedFileName + fileExtension;
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserVM userVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);

            user.FullName = userVM.User.FullName;
            user.Bio = userVM.User.Bio;
            user.Address = userVM.User.Address;
            user.DOB = userVM.User.DOB;
            user.Gender = userVM.User.Gender;
            user.Description = userVM.User.Description;

            await _userService.UpdateUserAsync(user);
            return Redirect("Index");
        }

        public async Task<IActionResult> PostHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetUserWithVotesAsync(userId);
            var posts = await _userService.GetUserPostsAsync(userId);

            userVM = new UserVM()
            {
                User = user,
                Post = posts
            };

            ViewData["CurrentPage"] = Url.Action("PostHistory");
            return View(userVM);
        }

        public async Task<IActionResult> CourseHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetUserWithVotesAsync(userId);
            var enrollments = await _userService.GetUserEnrollmentsAsync(userId);

            userVM = new UserVM()
            {
                User = user,
                Enrollments = enrollments,
            };

            ViewData["CurrentPage"] = Url.Action("CourseHistory");
            return View(userVM);
        }
    }
}
