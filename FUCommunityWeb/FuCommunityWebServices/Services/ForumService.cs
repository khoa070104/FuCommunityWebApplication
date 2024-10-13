using FuCommunityWebDataAccess.Repositories;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebServices.Services
{
    public class ForumService
    {
        private readonly ForumRepo _forumRepo;

        public ForumService(ForumRepo forumRepo)
        {
            _forumRepo = forumRepo;
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _forumRepo.GetAllPostsAsync();
        }
        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _forumRepo.GetAllCategoryAsync();
        }
        public async Task<(List<Post> posts, int totalItems)> GetPostsAsync(int page, int pageSize, string searchString)
        {
            return await _forumRepo.GetPostsAsync(page, pageSize, searchString);

        }
        public async Task<(List<Post> posts, int totalItems)> GetPostsByCategory(int categoryID, int page, int pageSize, string searchString)
        {
            return await _forumRepo.GetPostsByCategory(categoryID, page, pageSize, searchString);
        }

        public async Task<Post> GetPostByID(int id)
        {
            return await _forumRepo.GetPostByID(id);
        }

        public async Task<PostVM> GetComments(int postID)
        {
            return await _forumRepo.GetPostDetailsAsync(postID);
        }
    }
}
