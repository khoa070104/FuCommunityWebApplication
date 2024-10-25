﻿using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using FuCommunityWebModels.ViewModels.FuCommunityWebModels.ViewModels;
using FuCommunityWebServices.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using FuCommunityWebDataAccess.Data;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace FUCommunityWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly CourseService _courseService;
        private readonly ILogger<AdminController> _logger;
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly ApplicationDbContext _context;
        private readonly ForumService _forumService;

        public AdminController(CourseService courseService, UserService userService, ILogger<AdminController> logger, OrderService orderService, ApplicationDbContext context, ForumService forumService)
        {
            _courseService = courseService;
            _userService = userService;
            _logger = logger;
            _orderService = orderService;
            _context = context;
            _forumService = forumService;
        }

        public async Task<IActionResult> ManageCourse(string search, string sortOrder)
        {
            var courses = await _courseService.GetAllCoursesAsync();

            if (!string.IsNullOrEmpty(search))
            {
                courses = courses.Where(c => c.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewData["SearchQuery"] = search;
            ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["UserIDSortParm"] = sortOrder == "UserID" ? "userid_desc" : "UserID";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";

            switch (sortOrder)
            {
                case "id_desc":
                    courses = courses.OrderByDescending(c => c.CourseID).ToList();
                    break;
                case "UserID":
                    courses = courses.OrderBy(c => c.UserID).ToList();
                    break;
                case "userid_desc":
                    courses = courses.OrderByDescending(c => c.UserID).ToList();
                    break;
                case "Title":
                    courses = courses.OrderBy(c => c.Title).ToList();
                    break;
                case "title_desc":
                    courses = courses.OrderByDescending(c => c.Title).ToList();
                    break;
                default:
                    courses = courses.OrderBy(c => c.CourseID).ToList();
                    break;
            }

            var viewModel = new CourseVM
            {
                Courses = courses,
                Categories = await _courseService.GetAllCategoriesAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCourseVM createCourseVM)
        {
            if (ModelState.IsValid)
            {
                createCourseVM.CourseImage = await UploadCourseImage(createCourseVM.CourseImageFile);

                try
                {
                    var course = new Course
                    {
                        Title = createCourseVM.Title,
                        Description = createCourseVM.Description,
                        Price = createCourseVM.Price,
                        CourseImage = createCourseVM.CourseImage,
                        Status = createCourseVM.Status,
                        UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Semester = createCourseVM.Semester,
                        CategoryID = createCourseVM.CategoryID,
                        CreatedDate = DateTime.Now
                    };

                    await _courseService.AddCourseAsync(course);

                    _logger.LogInformation("Khóa học mới đã được tạo: {CourseTitle}", course.Title);
                    TempData["Success"] = "Tạo khóa học thành công.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo khóa học");
                    TempData["Error"] = "Đã xảy ra lỗi khi tạo khóa học. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return RedirectToAction("ManageCourse");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(EditCourseVM editCourseVM)
        {
            if (ModelState.IsValid)
            {
                var courseToUpdate = await _courseService.GetCourseByIdAsync(editCourseVM.CourseID);
                if (courseToUpdate == null)
                {
                    TempData["Error"] = "Khóa học không tồn tại.";
                    return RedirectToAction("ManageCourse");
                }

                courseToUpdate.Title = editCourseVM.Title;
                courseToUpdate.Description = editCourseVM.Description;
                courseToUpdate.Price = editCourseVM.Price;
                courseToUpdate.Status = editCourseVM.Status;
                courseToUpdate.Semester = editCourseVM.Semester;
                courseToUpdate.CategoryID = editCourseVM.CategoryID;
                if (editCourseVM.CourseImageFile != null && editCourseVM.CourseImageFile.Length > 0)
                {
                    courseToUpdate.CourseImage = await UploadCourseImage(editCourseVM.CourseImageFile);
                }

                courseToUpdate.UpdatedDate = DateTime.Now;

                try
                {
                    await _courseService.UpdateCourseAsync(courseToUpdate);
                    _logger.LogInformation("Khóa học đã được cập nhật: {CourseTitle}", courseToUpdate.Title);
                    TempData["Success"] = "Cập nhật khóa học thành công.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật khóa học");
                    TempData["Error"] = "Đã xảy ra lỗi khi cập nhật khóa học. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return RedirectToAction("ManageCourse");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);

            if (course == null)
            {
                TempData["Error"] = "Khóa học không tồn tại.";
                return RedirectToAction("ManageCourse");
            }

            try
            {
                await _courseService.DeleteCourseAsync(course);
                _logger.LogInformation("Khóa học đã được xóa: {CourseTitle}", course.Title);
                TempData["Success"] = "Xóa khóa học thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khóa học");
                TempData["Error"] = "Đã xảy ra lỗi khi xóa khóa học. Vui lòng thử lại.";
            }

            return RedirectToAction("ManageCourse");
        }

        private async Task<string> UploadCourseImage(IFormFile courseImageFile)
        {
            if (courseImageFile != null && courseImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(courseImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await courseImageFile.CopyToAsync(fileStream);
                }

                return "/uploads/" + uniqueFileName;
            }
            return "/img/Logo_FunnyCode.jpg";
        }

        public async Task<IActionResult> ManageLesson(int courseId, string search, string sortOrder)
        {
            var lessons = await _courseService.GetLessonsByCourseIdAsync(courseId);

            if (lessons == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(search))
            {
                lessons = lessons.Where(l => l.Title.Contains(search) || l.LessonID.ToString().Contains(search)).ToList();
            }
            ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";

            switch (sortOrder)
            {
                case "id_desc":
                    lessons = lessons.OrderByDescending(l => l.LessonID).ToList();
                    break;
                case "Title":
                    lessons = lessons.OrderBy(l => l.Title).ToList();
                    break;
                case "title_desc":
                    lessons = lessons.OrderByDescending(l => l.Title).ToList();
                    break;
                default:
                    lessons = lessons.OrderBy(l => l.LessonID).ToList();
                    break;
            }

            var course = await _courseService.GetCourseByIdAsync(courseId); 

            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseDetailVM
            {
                Course = course,
                Lessons = lessons,
                CreateLessonVM = new CreateLessonVM { CourseID = courseId }, 
                ShowCreateLessonModal = false,
                ShowEditLessonModal = false
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(CreateLessonVM createLessonVM)
        {
            var course = await _courseService.GetCourseByIdAsync(createLessonVM.CourseID); 

            if (course == null)
            {
                TempData["Error"] = "Khóa học không tồn tại.";
                return RedirectToAction("ManageLesson", new { courseId = createLessonVM.CourseID });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var lesson = new Lesson
                    {
                        Title = createLessonVM.Title,
                        Content = createLessonVM.Content,
                        Status = createLessonVM.Status,
                        CourseID = createLessonVM.CourseID, 
                        UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        CreatedDate = DateTime.Now
                    };

                    await _courseService.AddLessonAsync(lesson);

                    _logger.LogInformation("Bài học mới đã được tạo: {LessonTitle}", lesson.Title);
                    TempData["Success"] = "Tạo bài học thành công.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo bài học");
                    TempData["Error"] = "Đã xảy ra lỗi khi tạo bài học. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return RedirectToAction("ManageLesson", "Admin", new { courseId = createLessonVM.CourseID });
        }

        [HttpGet]
        public async Task<IActionResult> EditLesson(int id)
        {
            var lesson = await _courseService.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                TempData["Error"] = "Bài học không tồn tại.";
                return RedirectToAction("ManageLesson", new { courseId = lesson?.CourseID });
            }
            var editLessonVM = new EditLessonVM
            {
                LessonID = lesson.LessonID,
                CourseID = lesson.CourseID,
                Title = lesson.Title,
                Content = lesson.Content,
                Status = lesson.Status
            };

            var viewModel = new CourseDetailVM
            {
                EditLessonVM = editLessonVM
            };
            return PartialView("ManageLesson", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(EditLessonVM editLessonVM)
        {
            var lessonToUpdate = await _courseService.GetLessonByIdAsync(editLessonVM.LessonID);

            if (lessonToUpdate == null)
            {
                TempData["Error"] = "Bài học không tồn tại.";
                return RedirectToAction("ManageLesson", new { courseId = editLessonVM.CourseID });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    lessonToUpdate.Title = editLessonVM.Title;
                    lessonToUpdate.Content = editLessonVM.Content;
                    lessonToUpdate.Status = editLessonVM.Status;
                    lessonToUpdate.UpdatedDate = DateTime.Now;

                    await _courseService.UpdateLessonAsync(lessonToUpdate);

                    _logger.LogInformation("Bài học đã được cp nhật: {LessonTitle}", lessonToUpdate.Title);
                    TempData["Success"] = "Cập nhật bài học thành công.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật bài học");
                    TempData["Error"] = "Đã xảy ra lỗi khi cập nhật bài học. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return RedirectToAction("ManageLesson", new { courseId = lessonToUpdate.CourseID });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var lesson = await _courseService.GetLessonByIdAsync(lessonId);

            if (lesson == null)
            {
                TempData["Error"] = "Bài học không tồn tại.";
                return RedirectToAction("ManageLesson", new { courseId = lesson.CourseID });
            }

            try
            {
                await _courseService.DeleteLessonAsync(lesson);
                _logger.LogInformation("Bài học đã được xóa: {LessonTitle}", lesson.Title);
                TempData["Success"] = "Xóa bài học thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bài học");
                TempData["Error"] = "Đã xảy ra lỗi khi xóa bài học. Vui lòng thử lại.";
            }

            return RedirectToAction("ManageLesson", new { courseId = lesson.CourseID });
        }


        public async Task<IActionResult> AdminDashBoard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalPosts = await _context.Posts.CountAsync();
            var totalAmount = await _context.Orders
                .Where(o => o.Status == "1")
                .SumAsync(o => o.Amount);

            var userRegistrations = await _context.Users
                .GroupBy(u => new { u.CreatedDate.Year, u.CreatedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyRegistrations = new int[12];

            for (int i = 0; i < 12; i++)
            {
                var month = (currentMonth - i + 11) % 12 + 1;
                var year = currentMonth - i <= 0 ? currentYear - 1 : currentYear;

                var registration = userRegistrations.FirstOrDefault(r => r.Year == year && r.Month == month);
                monthlyRegistrations[11 - i] = registration?.Count ?? 0;
            }

            var dashboardVM = new DashboardVM
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalPosts = totalPosts,
                TotalAmount = totalAmount,
                MonthlyUserRegistrations = monthlyRegistrations
            };

            return View(dashboardVM);
        }
        public IActionResult ManageForumGroup()
        {
            return View();
        }
        public async Task<IActionResult> ManagePost(CategoryVM categoryVM)
        {
            var modal = new PostVM
            {
                CategoryVM = new CategoryVM
                {
                    CategoryID = categoryVM.CategoryID
                }
            };
            var posts = await _forumService.GetPostsByCategory(int.Parse(categoryVM.CategoryID));
            modal.Posts = posts;
            return View(modal);
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
                Type = postVM.CreatePostVM.Type,
                PostImage = postVM.CreatePostVM.PostImage
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManagePost", new
            {
                CategoryID = postVM.CreatePostVM.CategoryID
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
            return View(modal);
        }

        [HttpGet]
        public async Task<IActionResult> GetPost(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var postData = new
            {
                postID = post.PostID,
                title = post.Title,
                tag = post.Tag,
                type = post.Type,
                content = post.Content
            };

            return Json(postData);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostVM postVM)
        {
            var existingPost = await _forumService.GetPostByID(postVM.Post.PostID);
            if (existingPost == null)
            {
                return NotFound();
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
            existingPost.Type = postVM.CreatePostVM.Type;


            await _forumService.UpdatePost(existingPost);

            return RedirectToAction("ManagePost", new
            {
                CategoryID = postVM.CreatePostVM.CategoryID
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

            return RedirectToAction("ManagePost", new
            {
                categoryId = postVM.CategoryVM.CategoryID
            });
        }

        public IActionResult ManageForumPost()
        {
            return View();
        }
        public IActionResult ManageForumUser()
        {
            return View();
        }
        public async Task<IActionResult> ManagePayment()
        {
            var orders = await _orderService.GetAllOrdersAsync(); 
            return View(orders);
        }
        public IActionResult ManageStudent()
        {
            return View();
        }
        public async Task<IActionResult> ManageUser(string search, string sortOrder)
        {
            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.FullName.Contains(search) || u.Email.Contains(search)).ToList();
            }

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["PointSortParm"] = sortOrder == "Point" ? "point_desc" : "Point";

            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(u => u.FullName).ToList();
                    break;
                case "Email":
                    users = users.OrderBy(u => u.Email).ToList();
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email).ToList();
                    break;
                case "Point":
                    users = users.OrderBy(u => u.Point).ToList();
                    break;
                case "point_desc":
                    users = users.OrderByDescending(u => u.Point).ToList();
                    break;
                default:
                    users = users.OrderBy(u => u.FullName).ToList();
                    break;
            }

            var viewModel = new ManageUserVM
            {
                Users = users
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại.";
                return RedirectToAction("ManageUser");
            }

            try
            {
                await _userService.DeleteUserAsync(user);
                _logger.LogInformation("Đã xóa người dùng: {UserEmail}", user.Email);
                TempData["Success"] = "Xóa người dùng thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng");
                TempData["Error"] = "Đã xảy ra lỗi khi xóa người dùng. Vui lòng thử lại.";
            }

            return RedirectToAction("ManageUser");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(long orderId)
        {
            try
            {
                await _orderService.DeleteOrderAsync(orderId); 
                TempData["Success"] = "Order deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");
                TempData["Error"] = "An error occurred while deleting the order. Please try again.";
            }

            return RedirectToAction("ManagePayment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BanUser(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại.";
                return RedirectToAction("ManageUser");
            }

            try
            {
                user.Ban = true;
                await _userService.UpdateUserAsync(user);
                _logger.LogInformation("Người dùng đã bị ban: {UserEmail}", user.Email);
                TempData["Success"] = "Người dùng đã bị ban thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi ban người dùng");
                TempData["Error"] = "Đã xảy ra lỗi khi ban người dùng. Vui lòng thử lại.";
            }

            return RedirectToAction("ManageUser");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại.";
                return RedirectToAction("ManageUser");
            }

            try
            {
                user.Ban = false;
                await _userService.UpdateUserAsync(user);
                _logger.LogInformation("Người dùng đã được unban: {UserEmail}", user.Email);
                TempData["Success"] = "Người dùng đã được unban thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi unban người dùng");
                TempData["Error"] = "Đã xảy ra lỗi khi unban người dùng. Vui lòng thử lại.";
            }

            return RedirectToAction("ManageUser");
        }

        public async Task<IActionResult> ManageForumCategory()
        {
            var categories = await _forumService.GetAllCategoryAsync();
            var viewModel = new ForumVM
            {
                Categories = categories
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ManagePostDetail(int postId)
        {
            var post = await _forumService.GetPostByID(postId);
            if (post == null)
            {
                return NotFound();
            }

            var comments = await _forumService.GetCommentsByPostID(postId); 
            var viewModel = new PostVM
            {
                Post = post,
                Comments = comments 
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind(Prefix = "CreateCategory")] CreateCategoryVM createCategoryVM)
        {
            _logger.LogInformation("Bắt đầu tạo danh mục mới.");

            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState hợp lệ.");
                var category = new Category
                {
                    CategoryName = createCategoryVM.CategoryName,
                    Description = createCategoryVM.Description
                };

                try
                {
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Danh mục đã được lưu vào cơ sở dữ liệu.");
                    TempData["Success"] = "Tạo danh mục thành công.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo danh mục");
                    TempData["Error"] = "Đã xảy ra lỗi khi tạo danh mục. Vui lòng thử lại.";
                }
            }
            else
            {
                _logger.LogWarning("ModelState không hợp lệ.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Lỗi ModelState: {ErrorMessage}", error.ErrorMessage);
                }
                TempData["Error"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }

            _logger.LogInformation("Kết thúc tạo danh mục.");
            return RedirectToAction("ManageForumCategory");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            var editCategoryVM = new EditCategoryVM
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description
            };

            return View(editCategoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory([Bind(Prefix = "EditCategory")] EditCategoryVM editCategoryVM)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(editCategoryVM.CategoryID);
                if (category == null)
                {
                    TempData["Error"] = "Danh mục không tồn tại.";
                    return RedirectToAction("ManageForumCategory");
                }

                category.CategoryName = editCategoryVM.CategoryName;
                category.Description = editCategoryVM.Description;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật danh mục thành công.";
            }
            else
            {
                TempData["Error"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }
            return RedirectToAction("ManageForumCategory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                TempData["Error"] = "Danh mục không tồn tại.";
                return RedirectToAction("ManageForumCategory");
            }

            var relatedPosts = _context.Posts.Where(p => p.CategoryID == categoryId).ToList();

            if (relatedPosts.Any())
            {
                _context.Posts.RemoveRange(relatedPosts);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa danh mục thành công.";
            return RedirectToAction("ManageForumCategory");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(PostVM postVM)
        {
            var existingComment = await _forumService.GetCommentByID(postVM.Comment.CommentID);
            if (existingComment == null)
            {
                return NotFound();
            }

            await _forumService.DeteleComment(existingComment.CommentID);

            return RedirectToAction("ManagePostDetail", new
            {
                postId = postVM.Post.PostID
            });
        }


    }
}
