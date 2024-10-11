using FuCommunityWebDataAccess.Repositories;
using FuCommunityWebModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuCommunityWebServices.Services
{
    public class CourseService
    {
        private readonly CourseRepo _courseRepo;

        public CourseService(CourseRepo courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _courseRepo.GetAllCoursesAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _courseRepo.GetCourseByIdAsync(id);
        }

        public async Task AddCourseAsync(Course course)
        {
            await _courseRepo.AddCourseAsync(course);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await _courseRepo.UpdateCourseAsync(course);
        }

        public async Task DeleteCourseAsync(Course course)
        {
            await _courseRepo.DeleteCourseAsync(course);
        }

        public async Task<Lesson> GetLessonByIdAsync(int lessonId)
        {
            return await _courseRepo.GetLessonByIdAsync(lessonId);
        }

        public async Task AddLessonAsync(Lesson lesson)
        {
            await _courseRepo.AddLessonAsync(lesson);
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            await _courseRepo.UpdateLessonAsync(lesson);
        }

        public async Task DeleteLessonAsync(Lesson lesson)
        {
            await _courseRepo.DeleteLessonAsync(lesson);
        }

        public async Task<List<int>> GetEnrolledCoursesAsync(string userId)
        {
            return await _courseRepo.GetEnrolledCoursesAsync(userId);
        }

        public async Task<List<Course>> GetMostPurchasedCoursesAsync(int count)
        {
            return await _courseRepo.GetMostPurchasedCoursesAsync(count);
        }

        public async Task<List<Course>> GetHighestQualityCoursesAsync(int count)
        {
            return await _courseRepo.GetHighestQualityCoursesAsync(count);
        }

        public async Task<List<Course>> GetFilteredCoursesAsync(string semester, string category, string subjectCode, string minPrice)
        {
            return await _courseRepo.GetFilteredCoursesAsync(semester, category, subjectCode, minPrice);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _courseRepo.GetAllCategoriesAsync();
        }

        public async Task<List<string>> GetAllSubjectCodesAsync()
        {
            return await _courseRepo.GetAllSubjectCodesAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _courseRepo.GetUserByIdAsync(userId);
        }

        public async Task<Enrollment> GetEnrollmentAsync(string userId, int courseId)
        {
            return await _courseRepo.GetEnrollmentAsync(userId, courseId);
        }

        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            await _courseRepo.AddEnrollmentAsync(enrollment);
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            await _courseRepo.UpdateUserAsync(user);
        }

        public async Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId)
        {
            return await _courseRepo.GetLessonsByCourseIdAsync(courseId);
        }
    }
}