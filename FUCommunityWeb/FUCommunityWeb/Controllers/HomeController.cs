using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FuCommunityWebServices.Services;
using FuCommunityWebModels.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FuCommunityWebModels.ViewModels.FuCommunityWebModels.ViewModels;

namespace FUCommunityWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
		private readonly HomeService _homeService;

        public HomeController(ILogger<HomeController> logger, UserService userService, HomeService homeService, ApplicationDbContext context)
		{
			_logger = logger;
            _userService = userService;
            _homeService = homeService;
            _context = context;
        }
        public IActionResult Index()
		{
			return View();
		}

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }

        public async Task<IActionResult> Course(string semester, string category, string subjectCode, int? rate, decimal? minPrice)
        {
            // Lấy tất cả khóa học
            var courses = _context.Courses.AsQueryable();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = await _context.Enrollment
                .Where(e => e.UserID == userId)
                .Select(e => e.CourseID)
                .ToListAsync();

            // Áp dụng bộ lọc
            if (!string.IsNullOrEmpty(subjectCode))
            {
                courses = courses.Where(c => c.Title == subjectCode);
            }

            if (minPrice.HasValue)
            {
                courses = courses.Where(c => c.Price >= minPrice);
            }

            // Khởi tạo ViewModel
            var viewModel = new CourseVM
            {
                Courses = await courses.ToListAsync(),
                EnrolledCourses = enrolledCourses,
                CreateCourseVM = new CreateCourseVM(),
                EditCourseVM = new EditCourseVM(),
                ShowCreateCourseModal = false,
                ShowEditCourseModal = false,
                EditCourseID = null
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCourseVM createCourseVM)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload if provided
                if (createCourseVM.CourseImageFile != null && createCourseVM.CourseImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(createCourseVM.CourseImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await createCourseVM.CourseImageFile.CopyToAsync(fileStream);
                    }

                    createCourseVM.CourseImage = "/uploads/" + uniqueFileName;
                }
                else
                {
                    // Set default image if none is uploaded
                    createCourseVM.CourseImage = "/img/Logo_FunnyCode.jpg"; // Ensure this image exists
                }

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
                        CreatedDate = DateTime.Now
                    };

                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Course created successfully: {CourseTitle}", course.Title);

                    return RedirectToAction("Course");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating course");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the course. Please try again.");
                }
            }
            else
            {
                // Log ModelState errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError("Property: {Property}, Error: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }

                _logger.LogWarning("CreateCourse called with invalid ModelState.");
            }

            // If there are errors, reload courses and enrolled courses
            var courses = await _context.Courses.ToListAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = await _context.Enrollment
                .Where(e => e.UserID == userId)
                .Select(e => e.CourseID)
                .ToListAsync();

            var viewModel = new CourseVM
            {
                Courses = courses,
                EnrolledCourses = enrolledCourses,
                CreateCourseVM = createCourseVM, // Retain user input
                ShowCreateCourseModal = true
            };

            return View("Course", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Kiểm tra người dùng hiện tại có phải chủ sở hữu khóa học hay không
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.UserID != currentUserId)
            {
                return Forbid();
            }

            var editCourseVM = new EditCourseVM
            {
                CourseID = course.CourseID, // Đảm bảo CourseID được thiết lập
                Title = course.Title,
                Description = course.Description,
                Price = course.Price ?? 0,
                CourseImage = course.CourseImage,
                Status = course.Status
                // Note: CourseImageFile không được thiết lập, vì nó dành cho việc tải lên tệp mới
            };

            var viewModel = new CourseVM
            {
                EditCourseVM = editCourseVM,
                CreateCourseVM = new CreateCourseVM(), // Khởi tạo CreateCourseVM
                ShowEditCourseModal = true,
                EditCourseID = course.CourseID,
                Courses = await _context.Courses.ToListAsync(),
                EnrolledCourses = await _context.Enrollment
                    .Where(e => e.UserID == currentUserId)
                    .Select(e => e.CourseID)
                    .ToListAsync()
            };

            return View("Course", viewModel);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCourseVM editCourseVM)
        {
            if (!ModelState.IsValid)
            {
                // Lấy thông tin người dùng hiện tại
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Tìm khóa học trong cơ sở dữ liệu
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                // Đảm bảo người dùng là chủ sở hữu khóa học
                if (course.UserID != currentUserId)
                {
                    return Forbid();
                }

                // Đặt lại CourseImage trong ViewModel với đường dẫn hình ảnh hiện tại
                editCourseVM.CourseImage = course.CourseImage;

                // Tạo ViewModel để trả về View
                var viewModel = new CourseVM
                {
                    EditCourseVM = editCourseVM,
                    CreateCourseVM = new CreateCourseVM(), // Khởi tạo CreateCourseVM
                    ShowEditCourseModal = true,
                    EditCourseID = id,
                    Courses = await _context.Courses.ToListAsync(),
                    EnrolledCourses = await _context.Enrollment
                        .Where(e => e.UserID == currentUserId)
                        .Select(e => e.CourseID)
                        .ToListAsync()
                };
                return View("Course", viewModel);
            }

            var courseToUpdate = await _context.Courses.FindAsync(id);
            if (courseToUpdate == null)
            {
                return NotFound();
            }

            // Kiểm tra người dùng hiện tại có phải chủ sở hữu khóa học hay không
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (courseToUpdate.UserID != userId)
            {
                return Forbid();
            }

            // Cập nhật các thuộc tính của khóa học
            courseToUpdate.Title = editCourseVM.Title;
            courseToUpdate.Description = editCourseVM.Description;
            courseToUpdate.Price = editCourseVM.Price;
            courseToUpdate.Status = editCourseVM.Status;

            // Xử lý tải lên hình ảnh nếu có
            if (editCourseVM.CourseImageFile != null && editCourseVM.CourseImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(editCourseVM.CourseImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await editCourseVM.CourseImageFile.CopyToAsync(fileStream);
                }

                // Xóa hình ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(courseToUpdate.CourseImage))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseToUpdate.CourseImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                courseToUpdate.CourseImage = "/uploads/" + uniqueFileName;
            }

            courseToUpdate.UpdatedDate = DateTime.Now;

            try
            {
                _context.Courses.Update(courseToUpdate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Course edited successfully: {CourseTitle}", courseToUpdate.Title);

                return RedirectToAction("Course");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing course");
                ModelState.AddModelError(string.Empty, "An error occurred while editing the course. Please try again.");

                // Lấy lại CourseImage từ cơ sở dữ liệu nếu có lỗi
                var course = await _context.Courses.FindAsync(id);
                if (course != null)
                {
                    editCourseVM.CourseImage = course.CourseImage;
                }

                var updatedViewModel = new CourseVM
                {
                    EditCourseVM = editCourseVM,
                    CreateCourseVM = new CreateCourseVM(), // Khởi tạo CreateCourseVM
                    ShowEditCourseModal = true,
                    EditCourseID = id,
                    Courses = await _context.Courses.ToListAsync(),
                    EnrolledCourses = await _context.Enrollment
                        .Where(e => e.UserID == userId)
                        .Select(e => e.CourseID)
                        .ToListAsync()
                };

                return View("Course", updatedViewModel);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            // Lấy thông tin khóa học, bao gồm các thực thể liên quan
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Course");
            }

            // Lấy ID người dùng hiện tại
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra xem người dùng hiện tại có phải là chủ sở hữu khóa học không
            if (course.UserID != currentUserId)
            {
                TempData["Error"] = "You are not authorized to delete this course.";
                return Forbid(); // Hoặc bạn có thể redirect với một thông báo lỗi
            }

            try
            {
                // Xóa khóa học (Cascade Delete sẽ tự động xóa các thực thể liên quan)
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Course deleted successfully: {CourseTitle}", course.Title);
                TempData["Success"] = "Course deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course");
                TempData["Error"] = "An error occurred while deleting the course. Please try again.";
            }

            return RedirectToAction("Course");
        }



        public IActionResult CourseDetail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = _context.Enrollment
                .Where(e => e.UserID == userId)
                .Select(e => e.CourseID)
                .ToList();

            var courseDetail = _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.User)
                .FirstOrDefault(c => c.CourseID == id);

            if (courseDetail == null)
            {
                return NotFound();
            }

            var viewModel = new CourseDetailVM
            {
                Course = courseDetail,
                EnrolledCourses = enrolledCourses,
                Lessons = courseDetail.Lessons
                    .OrderBy(l => l.LessonID)
                    .ToList(),
                CreateLessonVM = new CreateLessonVM
                {
                    CourseID = id
                }
            };

            return View(viewModel);
        }

        // GET: Create Lesson
        [HttpGet]
        [Authorize]
        public IActionResult CreateLesson(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _context.Courses.Find(courseId);

            if (course == null)
            {
                return NotFound();
            }

            if (course.UserID != userId)
            {
                return Forbid();
            }

            var createLessonVM = new CreateLessonVM
            {
                CourseID = courseId
            };

            var viewModel = new CourseDetailVM
            {
                Course = course,
                CreateLessonVM = createLessonVM,
                ShowCreateLessonModal = true
            };

            return PartialView("_CreateLessonModal", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(CreateLessonVM createLessonVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _context.Courses.FindAsync(createLessonVM.CourseID);

            if (course == null)
            {
                return NotFound();
            }

            if (course.UserID != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var lesson = new Lesson
                {
                    CourseID = createLessonVM.CourseID,
                    UserID = userId,
                    Title = createLessonVM.Title,
                    Content = createLessonVM.Content,
                    Status = createLessonVM.Status,
                    CreatedDate = DateTime.Now
                };

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson created successfully: {LessonTitle}", lesson.Title);

                return RedirectToAction("CourseDetail", new { id = createLessonVM.CourseID });
            }

            // If we reach here, something failed; reload the CourseDetail view with validation errors
            var courseDetail = await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CourseID == createLessonVM.CourseID);

            var viewModel = new CourseDetailVM
            {
                Course = courseDetail,
                EnrolledCourses = _context.Enrollment
                    .Where(e => e.UserID == userId)
                    .Select(e => e.CourseID)
                    .ToList(),
                CreateLessonVM = createLessonVM,
                ShowCreateLessonModal = true
            };

            return View("CourseDetail", viewModel);
        }


        // GET: Edit Lesson
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditLesson(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lesson = await _context.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.LessonID == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            if (lesson.Course.UserID != userId)
            {
                return Forbid();
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
                Course = lesson.Course,
                EditLessonVM = editLessonVM,
                ShowEditLessonModal = true
            };

            return PartialView("_EditLessonModal", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int lessonId, EditLessonVM editLessonVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lesson = await _context.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.LessonID == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            if (lesson.Course.UserID != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                lesson.Title = editLessonVM.Title;
                lesson.Content = editLessonVM.Content;
                lesson.Status = editLessonVM.Status;
                lesson.UpdatedDate = DateTime.Now;

                try
                {
                    _context.Lessons.Update(lesson);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Lesson edited successfully: {LessonTitle}", lesson.Title);

                    return RedirectToAction("CourseDetail", new { id = lesson.CourseID });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing lesson");
                    ModelState.AddModelError(string.Empty, "An error occurred while editing the lesson. Please try again.");
                }
            }

            // If we reach here, something failed; reload the CourseDetail view with validation errors
            var courseDetail = await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CourseID == editLessonVM.CourseID);

            var viewModelFailure = new CourseDetailVM
            {
                Course = courseDetail,
                EnrolledCourses = _context.Enrollment
                    .Where(e => e.UserID == userId)
                    .Select(e => e.CourseID)
                    .ToList(),
                EditLessonVM = editLessonVM,
                ShowEditLessonModal = true
            };

            return View("CourseDetail", viewModelFailure);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lesson = await _context.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.LessonID == lessonId);

            if (lesson == null)
            {
                TempData["Error"] = "Lesson not found.";
                return RedirectToAction("CourseDetail", new { id = lesson.CourseID });
            }

            if (lesson.Course.UserID != userId)
            {
                TempData["Error"] = "You are not authorized to delete this lesson.";
                return Forbid();
            }

            try
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson deleted successfully: {LessonTitle}", lesson.Title);
                TempData["Success"] = "Lesson deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson");
                TempData["Error"] = "An error occurred while deleting the lesson. Please try again.";
            }

            return RedirectToAction("CourseDetail", new { id = lesson.CourseID });
        }




        public IActionResult CourseHistory()
        {
            return View();
        }
        public IActionResult Deposit()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        
        public IActionResult ForumDetail()
        {
            return View();
        }
        public IActionResult Home()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = _context.Enrollment
                .Where(e => e.UserID == userId)
                .Select(e => e.CourseID)
                .ToList();

            var mostPurchasedCourses = _context.Courses
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(3)
                .ToList();

            var highestQualityCourse = _context.Courses
                .OrderByDescending(c => c.Price)
                .Take(3)
                .ToList();

            var homeViewModel = new HomeVM
            {
                MostPurchasedCourses = mostPurchasedCourses,
                HighestQualityCourse = highestQualityCourse,
                EnrolledCourses = enrolledCourses
            };

            return View(homeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseDetails(int courseId, string returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Home");
            }

            var alreadyEnrolled = await _context.Enrollment
                .AnyAsync(e => e.UserID == userId && e.CourseID == courseId);

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Home");
                }
            }

            var enrollment = new Enrollment
            {
                UserID = userId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            _context.Enrollment.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment successful!";
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Home");
            }
        }

        public async Task<IActionResult> MentorHall()
        {
            var users = await _homeService.GetAllUsersWithVotesAsync();

            var sortedUsers = users
                .OrderByDescending(u => u.Point)
                .ToList();

            var topUsers = sortedUsers.Take(3).ToList();
            var otherUsers = sortedUsers.Skip(3).ToList();

            var mentorViewModel = new MentorVM
            {
                TopMentors = topUsers,
                OtherMentors = otherUsers
            };

            return View(mentorViewModel);
        }

        public async Task<IActionResult> Forum()
        {
            var courses = await _homeService.GetAllCoursesAsync();
            var posts = await _homeService.GetAllPostsAsync();

            var forumViewModel = new ForumVM
            {
                Courses = courses,
                Posts = posts
            };

            return View(forumViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(int page = 1, int pageSize = 2, string searchString = "")
        {
            var (posts, totalItems) = await _homeService.GetPostsAsync(page, pageSize, searchString);

            return Json(new
            {
                posts = posts,
                totalItems = totalItems,
                pageSize = pageSize,
                currentPage = page
            });
        }

        public IActionResult Post()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
