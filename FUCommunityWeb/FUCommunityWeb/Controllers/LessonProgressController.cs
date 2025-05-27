using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FuCommunityWebServices.Services;
using System.Security.Claims;

namespace FUCommunityWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonProgressController : ControllerBase
    {
        private readonly CourseService _courseService;

        public LessonProgressController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("complete/{lessonId}")]
        public async Task<IActionResult> MarkLessonAsCompleted(int lessonId, [FromBody] int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Kiểm tra user có enrolled trong course không
                var isEnrolled = await _courseService.IsUserEnrolledInCourseAsync(userId, courseId);
                if (!isEnrolled)
                {
                    return Forbid("Bạn chưa đăng ký khóa học này");
                }

                await _courseService.MarkLessonAsCompletedAsync(userId, lessonId, courseId);

                // Lấy thông tin tiến độ mới
                var completedCount = await _courseService.GetCompletedLessonsCountAsync(userId, courseId);
                var totalCount = await _courseService.GetTotalLessonsCountAsync(courseId);
                var progressPercentage = await _courseService.GetCourseProgressPercentageAsync(userId, courseId);

                return Ok(new
                {
                    success = true,
                    message = "Đã đánh dấu lesson hoàn thành",
                    completedCount = completedCount,
                    totalCount = totalCount,
                    progressPercentage = progressPercentage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("incomplete/{lessonId}")]
        public async Task<IActionResult> MarkLessonAsIncomplete(int lessonId, [FromBody] int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Kiểm tra user có enrolled trong course không
                var isEnrolled = await _courseService.IsUserEnrolledInCourseAsync(userId, courseId);
                if (!isEnrolled)
                {
                    return Forbid("Bạn chưa đăng ký khóa học này");
                }

                await _courseService.MarkLessonAsIncompleteAsync(userId, lessonId);

                // Lấy thông tin tiến độ mới
                var completedCount = await _courseService.GetCompletedLessonsCountAsync(userId, courseId);
                var totalCount = await _courseService.GetTotalLessonsCountAsync(courseId);
                var progressPercentage = await _courseService.GetCourseProgressPercentageAsync(userId, courseId);

                return Ok(new
                {
                    success = true,
                    message = "Đã bỏ đánh dấu lesson hoàn thành",
                    completedCount = completedCount,
                    totalCount = totalCount,
                    progressPercentage = progressPercentage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseProgress(int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var completedCount = await _courseService.GetCompletedLessonsCountAsync(userId, courseId);
                var totalCount = await _courseService.GetTotalLessonsCountAsync(courseId);
                var progressPercentage = await _courseService.GetCourseProgressPercentageAsync(userId, courseId);
                var lessonProgresses = await _courseService.GetUserLessonProgressByCourseAsync(userId, courseId);

                return Ok(new
                {
                    completedCount = completedCount,
                    totalCount = totalCount,
                    progressPercentage = progressPercentage,
                    lessonProgresses = lessonProgresses.Select(lp => new
                    {
                        lessonId = lp.LessonID,
                        isCompleted = lp.IsCompleted,
                        completedDate = lp.CompletedDate
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
