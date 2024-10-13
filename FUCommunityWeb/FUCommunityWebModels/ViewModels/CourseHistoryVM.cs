using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuCommunityWebModels.Models;

namespace FuCommunityWebModels.ViewModels
{
    public class CourseHistoryVM
    {
        public ApplicationUser User { get; set; } // Thông tin người dùng
        public List<Enrollment> Enrollments { get; set; } // Danh sách đăng ký
    }

}
