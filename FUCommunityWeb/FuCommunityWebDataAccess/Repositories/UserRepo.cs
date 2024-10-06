﻿using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebDataAccess.Repositories
{
    public class UserRepo
    {
        private readonly ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddUserAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

		public async Task UpdateUserAsync(ApplicationUser user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}
        public async Task<ApplicationUser> GetUserWithVotesAsync(string userId)
        {
            return await _context.Users.Include(u => u.IsVotes)
                                       .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId)
        {
            return await _context.Posts
                                 .Where(p => p.UserID == userId)
                                 .Include(p => p.Comments)
                                 .Include(p => p.Votes)
                                 .Include(p => p.User)
                                 .OrderByDescending(p => p.CreatedDate)
                                 .ToListAsync();
        }

        public async Task<List<Enrollment>> GetUserEnrollmentsAsync(string userId)
        {
            return await _context.Enrollment
                                 .Where(e => e.UserID == userId)
                                 .Include(e => e.Course)
                                 .OrderByDescending(e => e.EnrollmentDate)
                                 .ToListAsync();
        }

        public async Task UpdateUserAvatarAsync(string userId, string avatarPath)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.AvatarImage = avatarPath; 
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
