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
        public async Task<(List<Post> posts, int totalItems)> GetPostsByCategory(int categoryID, int page, int pageSize, string searchString)
        {
            // Giả sử bạn có một ngữ cảnh DbContext để truy vấn dữ liệu từ cơ sở dữ liệu
            var query = _context.Posts.AsQueryable(); // Khởi tạo truy vấn với Posts

            // Lọc theo categoryID
            query = query.Where(post => post.CategoryID == categoryID);

            // Lọc theo searchString nếu nó không rỗng
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(post => post.Title.Contains(searchString) || post.Content.Contains(searchString));
            }

            // Lấy tổng số bài viết sau khi lọc
            var totalItems = await query.CountAsync();

            // Phân trang và lấy bài viết theo pageSize và page
            var posts = await query
                .OrderBy(post => post.CreatedDate) // Giả sử bạn có trường CreatedDate để sắp xếp
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
        }
        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _context.Categories.ToListAsync();
        }

    }
}