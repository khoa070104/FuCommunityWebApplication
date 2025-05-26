using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace FuCommunityWebServices.Services
{
    public class DashboardService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DashboardService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<DashboardData> GetDashboardDataAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new DashboardData();

            return await LoadDashboardDataAsync(userId);
        }

        private async Task<DashboardData> LoadDashboardDataAsync(string userId)
        {
            using var context = _contextFactory.CreateDbContext();

            var dashboardData = new DashboardData();

            try
            {
                // Load enrollments with courses
                var enrollments = await context.Enrollment
                    .Where(e => e.UserID == userId)
                    .Include(e => e.Course)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .ToListAsync();

                dashboardData.Enrollments = enrollments;

                // Load course progress data
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.Course != null)
                    {
                        var completedCount = await context.LessonProgresses
                            .CountAsync(lp => lp.UserID == userId &&
                                            lp.CourseID == enrollment.CourseID &&
                                            lp.IsCompleted);

                        var totalCount = await context.Lessons
                            .CountAsync(l => l.CourseID == enrollment.CourseID &&
                                           l.Status == "Active");

                        var progressPercentage = totalCount > 0 ?
                            Math.Round((double)completedCount / totalCount * 100, 1) : 0;

                        dashboardData.CourseProgress.Add(new CourseProgressData
                        {
                            CourseId = enrollment.CourseID,
                            CourseTitle = enrollment.Course.Title,
                            CompletedLessons = completedCount,
                            TotalLessons = totalCount,
                            ProgressPercentage = progressPercentage,
                            EnrollmentDate = enrollment.EnrollmentDate,
                            Status = enrollment.Status ?? "Active"
                        });
                    }
                }

                // Load upcoming lessons (incomplete lessons)
                var enrolledCourseIds = enrollments.Select(e => e.CourseID).ToList();

                if (enrolledCourseIds.Any())
                {
                    var allLessons = await context.Lessons
                        .Where(l => enrolledCourseIds.Contains(l.CourseID) && l.Status == "Active")
                        .Include(l => l.Course)
                        .OrderBy(l => l.CourseID)
                        .ThenBy(l => l.LessonID)
                        .ToListAsync();

                    var completedLessonIds = await context.LessonProgresses
                        .Where(lp => lp.UserID == userId && lp.IsCompleted)
                        .Select(lp => lp.LessonID)
                        .ToListAsync();

                    dashboardData.UpcomingLessons = allLessons
                        .Where(l => !completedLessonIds.Contains(l.LessonID))
                        .Take(10)
                        .ToList();
                }

                return dashboardData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DashboardService Error: {ex.Message}");
                return dashboardData; // Return partial data instead of throwing
            }
        }


    }

    public class DashboardData
    {
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<CourseProgressData> CourseProgress { get; set; } = new();
        public List<Lesson> UpcomingLessons { get; set; } = new();
    }

    public class CourseProgressData
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public double ProgressPercentage { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
