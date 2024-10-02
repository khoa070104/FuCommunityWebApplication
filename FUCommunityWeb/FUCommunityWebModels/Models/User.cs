using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace FuCommunityWebModels.Models
{
	public class User
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }

        public bool Active { get; set; } = true;

        public int Points { get; set; } = 0;

        [DataType(DataType.DateTime)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; }

        // Navigation properties
        public virtual UserInfo UserInfo { get; set; }  // A User can have one UserInfo
        public virtual ICollection<Post> Posts { get; set; }  // A User can create many Posts
        public virtual ICollection<Course> Courses { get; set; }  // A User can create many Courses
        public virtual ICollection<Question> Questions { get; set; }  // A User can create many Questions
        public virtual ICollection<Comment> Comments { get; set; }  // A User can create many Comments
        public virtual ICollection<Document> Documents { get; set; }  // A User can upload many Documents
        public virtual ICollection<Enrollment> Enrollments { get; set; }  // A User can enroll in many Courses
        public virtual ICollection<IsVote> Votes { get; set; }  // A User can vote on many Posts
    }
}
