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

        public async Task<ApplicationUser> GetUserByIdAsync(string userId, bool includeVotes = false)
        {
            return await _userRepo.GetUserByIdAsync(userId, includeVotes);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync(bool includeVotes = false)
        {
            return await _userRepo.GetAllUsersAsync(includeVotes);
        }

        public async Task SaveUserAsync(ApplicationUser user)
        {
            await _userRepo.SaveUserAsync(user);
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId)
        {
            return await _userRepo.GetUserPostsAsync(userId);
        }

        public async Task<List<Enrollment>> GetUserEnrollmentsAsync(string userId)
        {
            return await _userRepo.GetUserEnrollmentsAsync(userId);
        }

        public async Task UpdateUserAvatarAsync(string userId, string avatarPath)
        {
            await _userRepo.UpdateUserAvatarAsync(userId, avatarPath);
        }

        public async Task<ApplicationUser> GetUserWithVotesAsync(string userId)
        {
            return await _userRepo.GetUserWithVotesAsync(userId);
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            await _userRepo.UpdateUserAsync(user);
        }

        public async Task<bool> IsFollowingAsync(string userId, string followId)
        {
            return await _userRepo.IsFollowingAsync(userId, followId);
        }

        public async Task FollowUserAsync(string userId, string followId)
        {
            await _userRepo.FollowUserAsync(userId, followId);
        }

        public async Task UnfollowUserAsync(string userId, string followId)
        {
            await _userRepo.UnfollowUserAsync(userId, followId);
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            return await _userRepo.IsUserInRoleAsync(userId, role);
        }

        public async Task UpdateUserBannerAsync(string userId, string bannerPath)
        {
            await _userRepo.UpdateUserBannerAsync(userId, bannerPath);
        }

        public async Task CreateUserAsync(ApplicationUser user, string password)
        {
            // Assuming UserRepo handles password hashing or identity logic
            await _userRepo.CreateUserAsync(user, password);
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            await _userRepo.DeleteUserAsync(user);
        }

        public async Task<List<ApplicationUser>> GetFollowersAsync(string userId)
        {
            return await _userRepo.GetFollowersAsync(userId);
        }
    }
}
