using FuCommunityWebDataAccess.Data;
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
	}
}
