using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using FuCommunityWebModels.ViewModels.FuCommunityWebModels.ViewModels;
using FuCommunityWebServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FUCommunityWeb.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        private readonly ApplicationDbContext _context;

        public CourseController(CourseService courseService, ILogger<CourseController> logger, ApplicationDbContext context)
        {
            _courseService = courseService;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string semester, string category, string subjectCode, int? rate, decimal? minPrice)
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId);

            var categories = await _context.Categories.ToListAsync();

            if (!string.IsNullOrEmpty(subjectCode))
            {
                courses = courses.Where(c => c.Title == subjectCode).ToList();
            }

            if (minPrice.HasValue)
            {
                courses = courses.Where(c => c.Price >= minPrice).ToList();
            }

            var viewModel = new CourseVM
            {
                Courses = courses,
                EnrolledCourses = enrolledCourses,
                CreateCourseVM = new CreateCourseVM(),
                EditCourseVM = new EditCourseVM(),
                ShowCreateCourseModal = false,
                ShowEditCourseModal = false,
                EditCourseID = null,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM createCourseVM)
        {
            if (ModelState.IsValid)
            {
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
                    createCourseVM.CourseImage = "/img/Logo_FunnyCode.jpg";
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
                        CreatedDate = DateTime.Now,
                        Semester = createCourseVM.Semester,
                        CategoryID = createCourseVM.CategoryID
                    };

                    await _courseService.AddCourseAsync(course);

                    _logger.LogInformation("Course created successfully: {CourseTitle}", course.Title);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating course");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the course. Please try again.");
                }
            }
            var categories = await _context.Categories.ToListAsync();

            var courses = await _courseService.GetAllCoursesAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId);

            var viewModel = new CourseVM
            {
                Courses = courses,
                EnrolledCourses = enrolledCourses,
                CreateCourseVM = createCourseVM,
                ShowCreateCourseModal = true,
                Categories = categories
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.UserID != currentUserId)
            {
                return Forbid();
            }

            var editCourseVM = new EditCourseVM
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price ?? 0,
                CourseImage = course.CourseImage,
                Status = course.Status,
                Semester = course.Semester,
                CategoryID = course.CategoryID
            };

            // Lấy danh sách Categories
            var categories = await _context.Categories.ToListAsync();

            var viewModel = new CourseVM
            {
                EditCourseVM = editCourseVM,
                CreateCourseVM = new CreateCourseVM(),
                ShowEditCourseModal = true,
                EditCourseID = course.CourseID,
                Courses = await _courseService.GetAllCoursesAsync(),
                EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(currentUserId),
                Categories = categories // Truyền danh sách Categories vào ViewModel
            };

            return View("Index", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCourseVM editCourseVM)
        {
            if (!ModelState.IsValid)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                if (course.UserID != currentUserId)
                {
                    return Forbid();
                }

                editCourseVM.CourseImage = course.CourseImage;

                // Lấy danh sách Categories
                var categories = await _context.Categories.ToListAsync();

                var viewModel = new CourseVM
                {
                    EditCourseVM = editCourseVM,
                    CreateCourseVM = new CreateCourseVM(),
                    ShowEditCourseModal = true,
                    EditCourseID = id,
                    Courses = await _courseService.GetAllCoursesAsync(),
                    EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(currentUserId),
                    Categories = categories // Truyền danh sách Categories vào ViewModel
                };
                return View("Index", viewModel);
            }

            var courseToUpdate = await _courseService.GetCourseByIdAsync(id);
            if (courseToUpdate == null)
            {
                return NotFound();
            }

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
            courseToUpdate.Semester = editCourseVM.Semester;
            courseToUpdate.CategoryID = editCourseVM.CategoryID;

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
                await _courseService.UpdateCourseAsync(courseToUpdate);

                _logger.LogInformation("Course edited successfully: {CourseTitle}", courseToUpdate.Title);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing course");
                ModelState.AddModelError(string.Empty, "An error occurred while editing the course. Please try again.");

                var course = await _courseService.GetCourseByIdAsync(id);
                if (course != null)
                {
                    editCourseVM.CourseImage = course.CourseImage;
                }

                // Lấy lại danh sách Categories
                var categories = await _context.Categories.ToListAsync();

                var updatedViewModel = new CourseVM
                {
                    EditCourseVM = editCourseVM,
                    CreateCourseVM = new CreateCourseVM(),
                    ShowEditCourseModal = true,
                    EditCourseID = id,
                    Courses = await _courseService.GetAllCoursesAsync(),
                    EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId),
                    Categories = categories // Truyền danh sách Categories vào ViewModel
                };

                return View("Index", updatedViewModel);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (course.UserID != currentUserId)
            {
                TempData["Error"] = "You are not authorized to delete this course.";
                return Forbid();
            }

            try
            {
                await _courseService.DeleteCourseAsync(course);

                _logger.LogInformation("Course deleted successfully: {CourseTitle}", course.Title);
                TempData["Success"] = "Course deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course");
                TempData["Error"] = "An error occurred while deleting the course. Please try again.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId);

            var courseDetail = await _courseService.GetCourseByIdAsync(id);

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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(CreateLessonVM createLessonVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _courseService.GetCourseByIdAsync(createLessonVM.CourseID);

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

                await _courseService.AddLessonAsync(lesson);

                _logger.LogInformation("Lesson created successfully: {LessonTitle}", lesson.Title);

                return RedirectToAction("Detail", new { id = createLessonVM.CourseID });
            }

            var courseDetail = await _courseService.GetCourseByIdAsync(createLessonVM.CourseID);

            var viewModel = new CourseDetailVM
            {
                Course = courseDetail,
                EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId),
                CreateLessonVM = createLessonVM,
                ShowCreateLessonModal = true
            };

            return View("Detail", viewModel);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditLesson(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonID == id);

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
                Lessons = await _context.Lessons
                            .Where(l => l.CourseID == lesson.CourseID)
                            .OrderBy(l => l.LessonID)
                            .ToListAsync(),
                EditLessonVM = editLessonVM,
                ShowEditLessonModal = true,
                EditLessonID = lesson.LessonID,
                EnrolledCourses = await _context.Enrollment
                                        .Where(e => e.UserID == userId)
                                        .Select(e => e.CourseID)
                                        .ToListAsync()
            };

            return View("Detail", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int id, EditLessonVM editLessonVM)
        {
            if (id != editLessonVM.LessonID)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lessonToUpdate = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonID == id);

            if (lessonToUpdate == null)
            {
                return NotFound();
            }

            if (lessonToUpdate.Course.UserID != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    lessonToUpdate.Title = editLessonVM.Title;
                    lessonToUpdate.Content = editLessonVM.Content;
                    lessonToUpdate.Status = editLessonVM.Status;
                    lessonToUpdate.UpdatedDate = DateTime.Now;

                    _context.Lessons.Update(lessonToUpdate);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Lesson edited successfully: {LessonTitle}", lessonToUpdate.Title);

                    return RedirectToAction("Detail", new { id = lessonToUpdate.CourseID });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing lesson");
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi chỉnh sửa bài học. Vui lòng thử lại.");
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

                _logger.LogWarning("EditLesson called with invalid ModelState.");
            }

            // Nếu có lỗi, tải lại thông tin khóa học và bài học
            var courseDetail = await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CourseID == lessonToUpdate.CourseID);

            var viewModelError = new CourseDetailVM
            {
                Course = courseDetail,
                EnrolledCourses = await _context.Enrollment
                                        .Where(e => e.UserID == userId)
                                        .Select(e => e.CourseID)
                                        .ToListAsync(),
                Lessons = courseDetail.Lessons.OrderBy(l => l.LessonID).ToList(),
                EditLessonVM = editLessonVM,
                ShowEditLessonModal = true,
                EditLessonID = lessonToUpdate.LessonID
            };

            return View("Detail", viewModelError);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lesson = await _courseService.GetLessonByIdAsync(lessonId);

            if (lesson == null)
            {
                TempData["Error"] = "Bài học không tồn tại.";
                return RedirectToAction("Detail", new { id = lesson.CourseID });
            }

            if (lesson.Course.UserID != userId)
            {
                TempData["Error"] = "Bạn không có quyền xóa bài học này.";
                return Forbid();
            }

            try
            {
                await _courseService.DeleteLessonAsync(lesson);

                _logger.LogInformation("Lesson deleted successfully: {LessonTitle}", lesson.Title);
                TempData["Success"] = "Bài học đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson");
                TempData["Error"] = "Đã xảy ra lỗi khi xóa bài học. Vui lòng thử lại.";
            }

            return RedirectToAction("Detail", new { id = lesson.CourseID });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyCourse(int courseId, string returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }

            var alreadyEnrolled = await _context.Enrollment
                .AnyAsync(e => e.UserID == userId && e.CourseID == courseId);

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Kiểm tra xem người dùng có đủ điểm để mua khóa học không
            if (user.Point < course.Price)
            {
                TempData["Error"] = "You do not have enough points to purchase this course.";
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            // Trừ điểm của người dùng
            user.Point -= course.Price.Value;

            var enrollment = new Enrollment
            {
                UserID = userId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            // Thêm đăng ký khóa học
            _context.Enrollment.Add(enrollment);
            await _context.SaveChangesAsync();

            // Cập nhật điểm của người dùng
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment successful!";

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}