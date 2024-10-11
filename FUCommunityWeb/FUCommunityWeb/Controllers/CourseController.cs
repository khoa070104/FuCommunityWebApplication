using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using FuCommunityWebServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using FuCommunityWebModels.ViewModels.FuCommunityWebModels.ViewModels; 

namespace FUCommunityWeb.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(CourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }


        public async Task<IActionResult> Index(string semester, string category, string subjectCode, string rate, string minPrice)
        {
            var viewModel = new CourseVM
            {
                Categories = await _courseService.GetAllCategoriesAsync(), 
                AllSubjectCodes = await _courseService.GetAllSubjectCodesAsync(), 
                SelectedSemester = semester,
                SelectedCategory = category,
                SelectedSubjectCode = subjectCode,
                SelectedRate = rate,
                SelectedMinPrice = minPrice
            };

            viewModel.Courses = await _courseService.GetFilteredCoursesAsync(semester, category, subjectCode, minPrice);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM createCourseVM)
        {
            if (ModelState.IsValid)
            {
                createCourseVM.CourseImage = await ProcessCourseImageAsync(createCourseVM.CourseImageFile);

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
            var categories = await _courseService.GetAllCategoriesAsync(); 
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


            var categories = await _courseService.GetAllCategoriesAsync(); 

            var viewModel = new CourseVM
            {
                EditCourseVM = editCourseVM,
                CreateCourseVM = new CreateCourseVM(),
                ShowEditCourseModal = true,
                EditCourseID = course.CourseID,
                Courses = await _courseService.GetAllCoursesAsync(), 
                EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(currentUserId), 
                Categories = categories
            };

            ModelState.Clear();

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
                editCourseVM.Semester = course.Semester;
                editCourseVM.CategoryID = course.CategoryID;

                ModelState.Remove("EditCourseVM.Semester");
                ModelState.Remove("EditCourseVM.CategoryID");

                var categories = await _courseService.GetAllCategoriesAsync();

                var viewModel = new CourseVM
                {
                  EditCourseVM = editCourseVM,
                  CreateCourseVM = new CreateCourseVM(),
                  ShowEditCourseModal = true,
                  EditCourseID = id,
                  Courses = await _courseService.GetAllCoursesAsync(), 
                  EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(currentUserId),
                  Categories = categories
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

            await HandleCourseUpdateAsync(courseToUpdate, editCourseVM);

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

                var categories = await _courseService.GetAllCategoriesAsync(); 

                var updatedViewModel = new CourseVM
                {
                  EditCourseVM = editCourseVM,
                  CreateCourseVM = new CreateCourseVM(),
                  ShowEditCourseModal = true,
                  EditCourseID = id,
                  Courses = await _courseService.GetAllCoursesAsync(), 
                  EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId), 
                  Categories = categories
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
                // Xóa ảnh liên quan đến khóa học
                await DeleteOldImageAsync(course.CourseImage);

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
            var lesson = await _courseService.GetLessonByIdAsync(id);

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
                Lessons = await _courseService.GetLessonsByCourseIdAsync(lesson.CourseID), 
                EditLessonVM = editLessonVM,
                ShowEditLessonModal = true,
                EditLessonID = lesson.LessonID,
                EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId)
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
            var lessonToUpdate = await _courseService.GetLessonByIdAsync(id);

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

                    await _courseService.UpdateLessonAsync(lessonToUpdate);

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
                
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError("Property: {Property}, Error: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }

                _logger.LogWarning("EditLesson called with invalid ModelState.");
            }

            var courseDetail = await _courseService.GetCourseByIdAsync(lessonToUpdate.CourseID);

            var viewModelError = new CourseDetailVM
            {
                Course = courseDetail,
                EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId),
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
            var course = await _courseService.GetCourseByIdAsync(courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }

            var alreadyEnrolled = await _courseService.GetEnrollmentAsync(userId, courseId) != null;

            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction("Index");
            }

            var user = await _courseService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            if (user.Point < course.Price)
            {
                TempData["Error"] = "You do not have enough points to purchase this course.";
                return RedirectToAction("Index");
            }

            user.Point -= course.Price.Value;

            var enrollment = new Enrollment
            {
                UserID = userId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            await _courseService.AddEnrollmentAsync(enrollment);
            await _courseService.UpdateUserAsync(user);

            TempData["Success"] = "Enrollment successful!";
            return RedirectToAction("Index");
        }

        private string GenerateUniqueFileName(IFormFile file)
        {
            var courseKey = Guid.NewGuid().ToString(); // Hoặc sử dụng một khóa cụ thể cho khóa học
            var extension = Path.GetExtension(file.FileName);
            return $"course_{courseKey}{extension}";
        }

        private async Task DeleteOldImageAsync(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
        }

        private async Task<string> ProcessCourseImageAsync(IFormFile courseImageFile)
        {
            if (courseImageFile != null && courseImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = GenerateUniqueFileName(courseImageFile);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await courseImageFile.CopyToAsync(fileStream);
                }

                return "/uploads/" + uniqueFileName;
            }
            return "/img/Logo_FunnyCode.jpg"; // Default image
        }

        private async Task HandleCourseUpdateAsync(Course courseToUpdate, EditCourseVM editCourseVM)
        {
            courseToUpdate.Title = editCourseVM.Title;
            courseToUpdate.Description = editCourseVM.Description;
            courseToUpdate.Price = editCourseVM.Price;
            courseToUpdate.Status = editCourseVM.Status;
            courseToUpdate.Semester = editCourseVM.Semester;
            courseToUpdate.CategoryID = editCourseVM.CategoryID;

            if (editCourseVM.CourseImageFile != null && editCourseVM.CourseImageFile.Length > 0)
            {
                // Xóa ảnh cũ
                await DeleteOldImageAsync(courseToUpdate.CourseImage);
                courseToUpdate.CourseImage = await ProcessCourseImageAsync(editCourseVM.CourseImageFile);
            }

            courseToUpdate.UpdatedDate = DateTime.Now;
        }
    }
}