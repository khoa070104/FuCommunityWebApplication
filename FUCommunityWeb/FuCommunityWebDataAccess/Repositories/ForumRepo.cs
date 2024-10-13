using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebDataAccess.Repositories
{
    public class ForumRepo
    {
        private readonly ApplicationDbContext _context;
        public ForumRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PostVM> GetPostDetailsAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            var comments = await _context.Comments
                .Where(c => c.PostID == postId)
                .Select(c => new Comment
                {
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    UserID = c.UserID
                }).ToListAsync();

            var userIds = comments.Select(c => c.UserID).Distinct().ToList();

            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new UserVM
                {
                    User = u,  
                    Post = _context.Posts.Where(p => p.UserID == u.Id).ToList()
                }).ToListAsync();

            return new PostVM
            {
                Post = post,
                Comments = comments,
                Users = users
            };
        }


        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<(List<Post> posts, int totalItems)> GetPostsByCategory(int categoryID, int page, int pageSize, string searchString)
        {
            var query = _context.Posts.AsQueryable(); 

            query = query.Where(post => post.CategoryID == categoryID);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(post => post.Title.Contains(searchString) || post.Content.Contains(searchString));
            }

            var totalItems = await query.CountAsync();

            var posts = await query
                .OrderBy(post => post.PostID) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
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
        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }
        public async Task<Post> GetPostByID(int postId)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(post => post.PostID == postId);
        }
    }
}
