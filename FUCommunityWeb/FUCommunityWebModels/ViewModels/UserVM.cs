using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FuCommunityWebModels.ViewModels
{
    public class UserVM
    {
        public ApplicationUser User { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Post> Post { get; set; }
        //public string FullName { get; set; }
        //public string Bio { get; set; }
        //public string Address { get; set; }
        //public string Gender { get; set; }
        //[ValidateNever]
        //public DateTime DOB { get; set; }
        //public string PhoneNumber { get; set; }
        //[ValidateNever]
        //public string Description { get; set; }
        //[ValidateNever]
        //public string AvatarImage { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsFollowing { get; set; }

        // Thêm thuộc tính để lưu trữ danh sách người theo dõi
        public List<ApplicationUser> Followers { get; set; }
        public bool IsMentor { get; set; }

        // Thêm các thuộc tính mới để lưu trữ tổng số Posts và Questions
        public int TotalPosts { get; set; }
        public int TotalQuestions { get; set; }
    }
}
