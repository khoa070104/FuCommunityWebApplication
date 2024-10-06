using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuCommunityWebDataAccess.Repositories
{
    public class HomeRepo 
    {
        private readonly ApplicationDbContext _context;

        public HomeRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<(List<Post> posts, int totalItems)> GetPostsAsync(int page, int pageSize, string searchString)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Title.Contains(searchString) || p.Content.Contains(searchString));
            }

            var totalItems = await query.CountAsync();

            var posts = await query
                .OrderBy(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
        }

        public async Task<List<ApplicationUser>> GetAllUsersWithVotesAsync()
        {
            return await _context.Users
                .Include(u => u.IsVotes)
                .ToListAsync();
        }
    }
}