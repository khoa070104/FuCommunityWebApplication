using System;
using System.Collections.Generic;
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
		public string FullName { get; set; }

		[MaxLength(100)]
		public string Email { get; set; }

		[MaxLength(15)]
		public string Phone { get; set; }

		public char? Gender { get; set; }

		[DataType(DataType.Date)]
		public DateTime? DOB { get; set; }

		public string Bio { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? RegistrationDate { get; set; } = DateTime.Now;

		[DataType(DataType.DateTime)]
		public DateTime? LastLogin { get; set; }

		[MaxLength(255)]
		public string AvatarImage { get; set; }

		public bool? Active { get; set; } = true;
		public int Role { get; set; } = 2;
		public ICollection<Point> Points { get; set; } = null;
		public ICollection<Post> Posts { get; set; }  // Một User có nhiều Posts
		public ICollection<Comment> Comments { get; set; }  // Một User có nhiều Comments
		public ICollection<Course> Courses { get; set; }  // Một User có nhiều Courses
		public ICollection<Document> Documents { get; set; }  // Một User có nhiều Documents
		public ICollection<Question> Questions { get; set; }  // Một User có nhiều Questions
		public ICollection<Vote> Votes { get; set; }  // Một User có nhiều Votes
		public ICollection<Point> PointsEarned { get; set; }
	}
}
