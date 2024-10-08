using FuCommunityWebModels.Models;
using System.Collections.Generic;

namespace FuCommunityWebModels.ViewModels
{
    public class CourseVM
    {
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<int> EnrolledCourses { get; set; } = new List<int>();
        public CreateCourseVM CreateCourseVM { get; set; } = new CreateCourseVM();
        public EditCourseVM EditCourseVM { get; set; } = new EditCourseVM(); 
        public bool ShowCreateCourseModal { get; set; }
        public bool ShowEditCourseModal { get; set; } 
        public int? EditCourseID { get; set; } // To identify which course is being edited
    }
}
