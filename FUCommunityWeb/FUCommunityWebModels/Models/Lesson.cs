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

        public int CourseID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(255)]
        public string LessonUrl { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("CourseID")]
        public Course Course { get; set; }
    }
}
