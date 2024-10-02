using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.Models
{
	public class Lesson
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonID { get; set; }

        public int CourseID { get; set; } // Khóa ngoại

        [ForeignKey("CourseID")]
        public Course Course { get; set; } // Mối quan hệ với Course

        public string UserID { get; set; }

        public int CategoryID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        [StringLength(255)]
        public string PostImage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("Id")]
        public virtual ApplicationUser User { get; set; } // Thay đổi từ User thành ApplicationUser

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<IsVote> Votes { get; set; }
    }
}
