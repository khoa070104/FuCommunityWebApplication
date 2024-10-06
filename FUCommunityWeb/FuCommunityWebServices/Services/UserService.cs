﻿using System;
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

        public async Task<ApplicationUser> GetUserWithVotesAsync(string userId)
        {
            return await _userRepo.GetUserWithVotesAsync(userId);
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId)
        {
            return await _userRepo.GetUserPostsAsync(userId);
        }

        public async Task<List<Enrollment>> GetUserEnrollmentsAsync(string userId)
        {
            return await _userRepo.GetUserEnrollmentsAsync(userId);
        }

        public async Task UpdateUserAvatarAsync(string? userId, string avatarPath)
        {
            await _userRepo.UpdateUserAvatarAsync(userId, avatarPath);
        }
    }
}
