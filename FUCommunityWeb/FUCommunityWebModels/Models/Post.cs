using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace FuCommunityWebModels.Models
{
	public class Post
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PostID { get; set; }

		public int? UserID { get; set; }
		public int? CategoryID { get; set; }

		[Required]
		[MaxLength(255)]
		public string Title { get; set; }

		public string Content { get; set; }

		[MaxLength(255)]
		public string PostImage { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? CreatedDate { get; set; } = DateTime.Now;

		[DataType(DataType.DateTime)]
		public DateTime? UpdatedDate { get; set; }

		[MaxLength(50)]
		public string Status { get; set; }

		// Navigation properties
		[ForeignKey("CategoryID")]
		public Category Category { get; set; }

		[ForeignKey("UserID")]
		public User User { get; set; }

		public ICollection<Comment> Comments { get; set; }  // Một Post có nhiều Comments
		public ICollection<Document> Documents { get; set; }  // Một Post có nhiều Documents
		public ICollection<Vote> Votes { get; set; }  // Một Post có nhiều Votes
		public ICollection<Point> Points { get; set; }
	}
}
