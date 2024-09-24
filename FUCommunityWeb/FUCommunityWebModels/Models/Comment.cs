using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.Models
{
	public class Comment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CommentID { get; set; }

		public int? PostID { get; set; }
		public int? QuestionID { get; set; }
		public int? UserID { get; set; }

		public string Content { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? CreatedDate { get; set; } = DateTime.Now;

		[DataType(DataType.DateTime)]
		public DateTime? UpdatedDate { get; set; }

		// Navigation properties
		[ForeignKey("PostID")]
		public Post Post { get; set; }

		[ForeignKey("QuestionID")]
		public Question Question { get; set; }

		[ForeignKey("UserID")]
		public User User { get; set; }
	}
}
