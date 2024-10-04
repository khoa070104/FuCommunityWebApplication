using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FuCommunityWebServices.Services;
using System.Threading.Tasks;
using FuCommunityWebModels.ViewModels;

namespace FUCommunityWeb.Controllers
{
	public class ProfileController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly UserService _userService; // Thêm UserService

		public ProfileController(UserManager<IdentityUser> userManager, UserService userService)
		{
			_userManager = userManager;
			_userService = userService; // Khởi tạo UserService
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
			return View(userVM);
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
