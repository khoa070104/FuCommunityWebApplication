using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.Models
{
    [Table("LessonProgress")]
    public class LessonProgress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonProgressID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int LessonID { get; set; }

        [Required]
        public int CourseID { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("LessonID")]
        public virtual Lesson Lesson { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }
    }
}
