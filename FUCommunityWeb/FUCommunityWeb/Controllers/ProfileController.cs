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
		private readonly UserService _userService; // Thêm UserService
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<IdentityUser> userManager, UserService userService, ApplicationDbContext context)
		{
			_userManager = userManager;
			_userService = userService; // Khởi tạo UserService
			_context = context;
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
				PhoneNumber = user.PhoneNumber,
				AvatarImage = user.AvatarImage,
			};
            ViewData["CurrentPage"] = Url.Action("Index");
            return View(userVM);
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
        [HttpPost]
		public async Task<IActionResult> Index(UserVM userVM)
		{
			//if (ModelState.IsValid)
			//{
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
			//}
			//return View(userVM);
			
		}
	}
}
