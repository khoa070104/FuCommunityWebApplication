using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace FuCommunityWebModels.Models
{
	public class Course
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CourseID { get; set; }

		public int? UserID { get; set; }

		[Required]
		[MaxLength(255)]
		public string Title { get; set; }

		public string Description { get; set; }

		[Column(TypeName = "decimal(10,2)")]
		public decimal? Price { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? CreatedDate { get; set; } = DateTime.Now;

		[DataType(DataType.DateTime)]
		public DateTime? UpdatedDate { get; set; }

		[MaxLength(255)]
		public string CourseImage { get; set; }

		[MaxLength(50)]
		public string Status { get; set; }

		// Navigation property
		[ForeignKey("UserID")]
		public User User { get; set; }

		public ICollection<Document> Documents { get; set; }  // Một Course có nhiều Documents
		public ICollection<Lesson> Lessons { get; set; }  // Một Course có nhiều Lessons
	}
}
