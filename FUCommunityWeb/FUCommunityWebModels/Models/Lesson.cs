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

		public int? CourseID { get; set; }

		[Required]
		[MaxLength(255)]
		public string Title { get; set; }

		public string Content { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? CreatedDate { get; set; } = DateTime.Now;

		[DataType(DataType.DateTime)]
		public DateTime? UpdatedDate { get; set; }

		[MaxLength(255)]
		public string LessonUrl { get; set; }

		[MaxLength(50)]
		public string Status { get; set; }

		// Navigation property
		[ForeignKey("CourseID")]
		public Course Course { get; set; }
	}
}
