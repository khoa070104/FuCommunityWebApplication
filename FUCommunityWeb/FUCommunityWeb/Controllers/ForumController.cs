using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using FuCommunityWebServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FUCommunityWeb.Controllers
{
    public class ForumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ForumService _forumService;
        private readonly HomeService _homeService;
        private readonly UserService _userService;

        public ForumController(ILogger<HomeController> logger, ApplicationDbContext context, ForumService forumService, HomeService homeService, UserService userService)
        {
            _logger = logger;
            _context = context;
            _forumService = forumService;
            _homeService = homeService;
            _userService = userService;
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
            var replyID = postVM.Comment.ReplyID;

            var post = new PostVM();

            if (replyID == null)
            {
                post.Comment = new Comment
                {
                    Content = WebUtility.HtmlEncode(postVM.Comment.Content),
                    PostID = postVM.Comment.PostID,
                    UserID = userId,
                    CreatedDate = DateTime.Now
                };
            }
            else
            {
                post.Comment = new Comment
                {
                    Content = WebUtility.HtmlEncode(postVM.Comment.Content),
                    PostID = postVM.Comment.PostID,
                    ReplyID = postVM.Comment.ReplyID,
                    UserID = userId,
                    CreatedDate = DateTime.Now
                };
            }

            _context.Comments.Add(post.Comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("PostDetail", new { postId = postVM.Comment.PostID });
        }


        [HttpGet]
        public async Task<IActionResult> GetPosts(int categoryID, int page = 1, int pageSize = 2, string searchString = "")
        {
            var (posts, totalItems) = await _forumService.GetPostsByCategory(categoryID, page, pageSize, searchString);

            var postData = new List<object>();

            foreach (var post in posts)
            {
                var user = await _userService.GetUserById(post.UserID);

                postData.Add(new
                {
                    postID = post.PostID,
                    title = post.Title,
                    createdDate = post.CreatedDate.ToString(),
                    content = post.Content,
                    tag = post.Tag,
                    type = post.Type == 1 ? "Blog" : "Question",
                    userAvatar = user?.AvatarImage ?? "/img/default-avatar.png"
                });
            }

            return Json(new
            {
                posts = postData,
                totalItems = totalItems,
                pageSize = pageSize,
                currentPage = page
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostVM postVM)
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
                postVM.CreatePostVM.PostImage = "/uploads/gay.png";
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
                Type = postVM.CreatePostVM.Type,
                PostImage = postVM.CreatePostVM.PostImage
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Post", new
            {
                CategoryName = postVM.CategoryVM.CategoryName,
                CategoryID = postVM.CategoryVM.CategoryID
            });
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
            modal.Post.User = await _userService.GetUserById(modal.Post.UserID);
            return View(modal);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostVM postVM)
        {
            var existingPost = await _forumService.GetPostByID(postVM.Post.PostID);
            if (existingPost == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != existingPost.UserID)
            {
                return Forbid();
            }

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
                existingPost.PostImage = "/uploads/" + uniqueFileName;
            }

            existingPost.Title = postVM.Post.Title;
            existingPost.Content = WebUtility.HtmlEncode(postVM.Post.Content);
            existingPost.Tag = postVM.Post.Tag;

            await _forumService.UpdatePost(existingPost);

            return RedirectToAction("Post", new
            {
                CategoryName = postVM.CategoryVM.CategoryName,
                CategoryID = postVM.CategoryVM.CategoryID
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(PostVM postVM)
        {
            var existingPost = await _forumService.GetPostByID(postVM.Post.PostID);
            if (existingPost == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != existingPost.UserID)
            {
                return Forbid();
            }

            await _forumService.DeletePost(existingPost.PostID);

            return RedirectToAction("Post", new
            {
                CategoryName = postVM.CategoryVM.CategoryName,
                CategoryID = postVM.CategoryVM.CategoryID
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(PostVM postVM)
        {
            var existingComment = await _forumService.GetCommentByID(postVM.Comment.CommentID);
            if (existingComment == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != existingComment.UserID)
            {
                return Forbid();
            }

            await _forumService.DeteleComment(existingComment.CommentID);

            return RedirectToAction("PostDetail", new
            {
                postId = postVM.Post.PostID
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(PostVM postVM)
        {
            var existingComment = await _forumService.GetCommentByID(postVM.Comment.CommentID);
            if (existingComment == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != existingComment.UserID)
            {
                return Forbid();
            }

            existingComment.Content = WebUtility.HtmlEncode(postVM.Post.Content);

            await _forumService.UpdateComment(existingComment);

            return RedirectToAction("PostDetail", new
            {
                postId = postVM.Post.PostID
            });
        }

        [HttpPost]
        public async Task<IActionResult> Vote([FromBody] PostVM postVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingVote = await _forumService.GetVoteByUserAndPost(userId, postVM.Post.PostID);

            var post = await _forumService.GetPostByID(postVM.Post.PostID);
            if (post == null)
            {
                return NotFound();
            }

            var postOwnerId = post.UserID;

            if (existingVote != null)
            {
                await _forumService.DeleteVote(existingVote);
                await _forumService.UpdateUserPoints(postOwnerId, -10, PointSource.Vote);
                return Ok(new { message = "Vote removed successfully!" });
            }
            else
            {
                var newVote = new IsVote
                {
                    UserID = userId,
                    PostID = postVM.Post.PostID,
                    Point = postVM.Point.PointValue
                };
                await _forumService.AddVote(newVote);
                await _forumService.UpdateUserPoints(postOwnerId, 10, PointSource.Vote);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> CheckVote(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasVoted = await _context.IsVotes
            .AnyAsync(v => v.PostID == postId && v.UserID == userId);

            return Ok(new { hasVoted });
        }
    }
}
