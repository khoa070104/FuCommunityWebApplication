using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.Models
{
    public class ApplicationUser : IdentityUser
    {

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }

        public DateTime DOB { get; set; }

        public string Bio { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string AvatarImage { get; set; }
        public decimal Point { get; set; }  

        // Navigation properties
        public virtual ICollection<Post> Posts { get; set; } // Mối quan hệ với Post
        public virtual ICollection<Enrollment> Enrollments { get; set; } // Mối quan hệ với Enrollment
        public virtual ICollection<Question> Questions { get; set; } // Mối quan hệ với Question
        public virtual ICollection<Document> Documents { get; set; } // Mối quan hệ với Document
        public virtual ICollection<Vote> Votes { get; set; } // Mối quan hệ với Vote
        public virtual ICollection<IsVote> IsVotes { get; set; } // Mối quan hệ với IsVote
        public virtual ICollection<Point> Points { get; set; } // Mối quan hệ với bảng Point

        public virtual ICollection<Comment> Comments { get; set; } // Mối quan hệ với Comment
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Deposit> Deposits { get; set; } // Mối quan hệ với Deposit

    }
}
