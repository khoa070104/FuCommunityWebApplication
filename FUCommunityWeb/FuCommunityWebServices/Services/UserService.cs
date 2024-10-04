using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuCommunityWebDataAccess.Repositories;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;

namespace FuCommunityWebServices.Services
{
    public class UserService
    {
        private readonly UserRepo _userRepo;

        public UserService(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userRepo.GetUserByIdAsync(userId);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userRepo.GetAllUsersAsync();
        }

        public async Task AddUserAsync(ApplicationUser user)
        {
            await _userRepo.AddUserAsync(user);
        }

		public async Task UpdateUserAsync(ApplicationUser user)
		{
			await _userRepo.UpdateUserAsync(user);
		}

	}
}
