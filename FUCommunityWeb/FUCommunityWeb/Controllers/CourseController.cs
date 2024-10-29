using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using FuCommunityWebModels.ViewModels;
using FuCommunityWebModels.ViewModels.FuCommunityWebModels.ViewModels;
using FuCommunityWebServices.Services;
using FuCommunityWebUtility;
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

        public CourseController(CourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        private async Task<CourseVM> PrepareCourseViewModel(CreateCourseVM createCourseVM = null, EditCourseVM editCourseVM = null, bool showCreateModal = false, bool showEditModal = false, int? editCourseId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return new CourseVM
            {
                Courses = await _courseService.GetAllCoursesAsync(),
                EnrolledCourses = await _courseService.GetEnrolledCoursesAsync(userId),
                Categories = await _courseService.GetAllCategoriesAsync(),
                CreateCourseVM = createCourseVM ?? new CreateCourseVM(),
                EditCourseVM = editCourseVM ?? new EditCourseVM(),
                ShowCreateCourseModal = showCreateModal,
                ShowEditCourseModal = showEditModal,
                EditCourseID = editCourseId
            };
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

        public async Task<IActionResult> Index(string semester, string category, string subjectCode, string rate, string minPrice)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isMentor = User.IsInRole(SD.Role_User_Mentor);
            var isStudent = User.IsInRole(SD.Role_User_Student);

            var allCourses = await _courseService.GetFilteredCoursesAsync(semester, category, subjectCode, rate, minPrice);
            var activeCourses = allCourses.Where(c => c.Status == "active").ToList(); 

            var viewModel = new CourseVM
            {
                Categories = await _courseService.GetAllCategoriesAsync(),
                AllSubjectCodes = await _courseService.GetAllSubjectCodesAsync(),
                SelectedSemester = semester,
                SelectedCategory = category,
                SelectedSubjectCode = subjectCode,
                SelectedRate = rate,
                SelectedMinPrice = minPrice,
                Courses = activeCourses,
                IsMentor = isMentor,
                IsStudent = isStudent
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM createCourseVM)
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
                        UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Status = "inactive",
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

            var viewModel = await PrepareCourseViewModel(createCourseVM, showCreateModal: true);
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
                Semester = course.Semester,
                CategoryID = course.CategoryID 
            };

            var viewModel = await PrepareCourseViewModel(editCourseVM: editCourseVM, showEditModal: true, editCourseId: course.CourseID);
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

                editCourseVM.CourseImage = course.CourseImage;
                editCourseVM.Semester = course.Semester;
                editCourseVM.CategoryID = course.CategoryID;

                ModelState.Remove("EditCourseVM.Semester");
                ModelState.Remove("EditCourseVM.CategoryID");

                var viewModel = await PrepareCourseViewModel(editCourseVM: editCourseVM, showEditModal: true, editCourseId: id);
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

            courseToUpdate.Title = editCourseVM.Title;
            courseToUpdate.Description = editCourseVM.Description;
            courseToUpdate.Price = editCourseVM.Price;
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

                _logger.LogInformation("Course edited successfully: {CourseTitle}", courseToUpdate.Title);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing course");
                ModelState.AddModelError(string.Empty, "An error occurred while editing the course. Please try again.");

                var viewModel = await PrepareCourseViewModel(editCourseVM: editCourseVM, showEditModal: true, editCourseId: id);
                return View("Index", viewModel);
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
                Lessons = courseDetail.Lessons?.OrderBy(l => l.LessonID).ToList() ?? new List<Lesson>(),
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
                Lesson lesson = new();
                lesson.CourseID = createLessonVM.CourseID;
                lesson.UserID = userId;
                lesson.Title = createLessonVM.Title;
                lesson.Content = createLessonVM.Content;
                lesson.Status = createLessonVM.Status;
                lesson.CreatedDate = DateTime.Now;

                try
                {
                    await _courseService.AddLessonAsync(lesson);
                    _logger.LogInformation("Lesson created successfully: {LessonTitle}", lesson.Title);
                    return RedirectToAction("Detail", new { id = createLessonVM.CourseID });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating lesson");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the lesson. Please try again.");
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
            var lesson = await _courseService.GetLessonWithCourseAsync(id);

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
                EnrolledCourses = await _courseService.GetEnrolledCourseIdsAsync(userId)
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
            var lessonToUpdate = await _courseService.GetLessonWithCourseAsync(id);

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
                    ModelState.AddModelError(string.Empty, "An error occurred while editing the lesson. Please try again.");
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
                EnrolledCourses = await _courseService.GetEnrolledCourseIdsAsync(userId),
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
                TempData["Error"] = "Lesson not found.";
                return RedirectToAction("Detail", new { id = lesson.CourseID });
            }

            if (lesson.Course.UserID != userId)
            {
                TempData["Error"] = "You are not authorized to delete this lesson.";
                return Forbid();
            }

            try
            {
                await _courseService.DeleteLessonAsync(lesson);

                _logger.LogInformation("Lesson deleted successfully: {LessonTitle}", lesson.Title);
                TempData["Success"] = "Lesson deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson");
                TempData["Error"] = "An error occurred while deleting the lesson. Please try again.";
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

            var alreadyEnrolled = await _courseService.IsUserEnrolledInCourseAsync(userId, courseId);

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
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            user.Point -= course.Price.Value;

            var enrollment = new Enrollment
            {
                UserID = userId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            await _courseService.EnrollUserInCourseAsync(enrollment);
            await _courseService.UpdateUserAsync(user);

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