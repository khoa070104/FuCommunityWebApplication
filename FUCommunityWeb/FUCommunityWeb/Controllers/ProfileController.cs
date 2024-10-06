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

        public ProfileController(UserManager<IdentityUser> userManager, UserService userService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);
            UserVM userVM = new()
            {
                FullName = user.FullName,
                DOB = user.DOB,
                Bio = user.Bio,
                Description = user.Description,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
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

            user.FullName = userVM.FullName;
            user.Bio = userVM.Bio;
            user.Address = userVM.Address;
            user.DOB = userVM.DOB;
            user.Gender = userVM.Gender;
            user.Description = userVM.Description;

            await _userService.UpdateUserAsync(user);
            return View(userVM);
        }

        public async Task<IActionResult> PostHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetUserWithVotesAsync(userId);
            var posts = await _userService.GetUserPostsAsync(userId);

            var viewModel = new PostHistoryVM
            {
                User = user,
                Post = posts
            };

            ViewData["CurrentPage"] = Url.Action("PostHistory");
            return View(viewModel);
        }

        public async Task<IActionResult> CourseHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetUserWithVotesAsync(userId);
            var enrollments = await _userService.GetUserEnrollmentsAsync(userId);

            var model = new CourseHistoryVM
            {
                User = user,
                Enrollments = enrollments
            };

            ViewData["CurrentPage"] = Url.Action("CourseHistory");
            return View(model);
        }
    }
}
