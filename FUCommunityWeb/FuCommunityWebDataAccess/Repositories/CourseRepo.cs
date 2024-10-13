using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuCommunityWebDataAccess.Repositories
{
    public class CourseRepo
    {
        private readonly ApplicationDbContext _context;

        public CourseRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CourseID == id);
        }

        public async Task AddCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(Course course)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Lesson> GetLessonByIdAsync(int lessonId)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonID == lessonId);
        }

        public async Task AddLessonAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(Lesson lesson)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetEnrolledCoursesAsync(string userId)
        {
            return await _context.Enrollment
                .Where(e => e.UserID == userId)
                .Select(e => e.CourseID)
                .ToListAsync();
        }

        public async Task<List<Course>> GetMostPurchasedCoursesAsync(int count)
        {
            return await _context.Courses
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Course>> GetHighestQualityCoursesAsync(int count)
        {
            return await _context.Courses
                .OrderByDescending(c => c.Price)
                .Take(count)
                .ToListAsync();
        }
    }
}