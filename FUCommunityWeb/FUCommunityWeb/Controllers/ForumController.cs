using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using FuCommunityWebServices.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace FUCommunityWeb.Controllers
{
    public class ForumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ForumService _forumService;
        private readonly HomeService _homeService;

        public ForumController(ILogger<HomeController> logger, ApplicationDbContext context, ForumService forumService, HomeService homeService)
        {
            _logger = logger;
            _context = context;
            _forumService = forumService;
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _forumService.GetAllPostsAsync();
            var category = await _forumService.GetAllCategoryAsync();

            var forumViewModel = new ForumVM
            {
                Posts = posts,
                Categories = category
            };

            return View(forumViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(PostVM postVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = new PostVM
            {
                Comment = new Comment
                {
                    Content = WebUtility.HtmlEncode(postVM.Comment.Content),
                    PostID = postVM.Comment.PostID,
                    UserID = userId,
                    CreatedDate = DateTime.Now
                }
            };

            _context.Comments.Add(post.Comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("PostDetail", new { postId = postVM.Comment.PostID });
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(int categoryID, int page = 1, int pageSize = 2, string searchString = "")
        {
            var (posts, totalItems) = await _forumService.GetPostsByCategory(categoryID, page, pageSize, searchString);

            return Json(new
            {
                posts = posts,
                totalItems = totalItems,
                pageSize = pageSize,
                currentPage = page
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostVM postVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (postVM.CreatePostVM.PostImageFile != null && postVM.CreatePostVM.PostImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(postVM.CreatePostVM.PostImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postVM.CreatePostVM.PostImageFile.CopyToAsync(fileStream);
                }
                postVM.CreatePostVM.PostImage = "/uploads/" + uniqueFileName;
            }
            else
            {
                postVM.CreatePostVM.PostImage = "/uploads/default-image.png";
            }
            var encodedContent = WebUtility.HtmlEncode(postVM.CreatePostVM.Content);

            var post = new Post
            {
                Title = WebUtility.HtmlEncode(postVM.CreatePostVM.Title),
                Content = encodedContent,
                CategoryID = postVM.CreatePostVM.CategoryID,
                UserID = userId,
                CreatedDate = DateTime.Now,
                Status = "Published",
                Tag = WebUtility.HtmlEncode(postVM.CreatePostVM.Tag),
                PostImage = postVM.CreatePostVM.PostImage
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Post(CategoryVM categoryVM)
        {
            var modal = new PostVM
            {
                CategoryVM = new CategoryVM
                {
                    CategoryName = categoryVM.CategoryName,
                    CategoryID = categoryVM.CategoryID
                }
            };
            return View(modal);
        }

        [HttpGet]
        public async Task<IActionResult> PostDetail(int postId)
        {
            var modal = new PostVM();
            modal = await _forumService.GetComments(postId);
            return View(modal);
        }

    }
}
