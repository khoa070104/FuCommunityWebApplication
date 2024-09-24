using FuCommunityWebModels.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Document
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int DocumentID { get; set; }

	public int? UserID { get; set; }
	public int? CourseID { get; set; }
	public int? PostID { get; set; }

	[MaxLength(255)]
	public string Name { get; set; }

	[MaxLength(255)]
	public string FileUrl { get; set; }

	[DataType(DataType.DateTime)]
	public DateTime? UploadedAt { get; set; } = DateTime.Now;

	// Navigation properties
	[ForeignKey("CourseID")]
	public Course Course { get; set; }

	[ForeignKey("PostID")]
	public Post Post { get; set; }

	[ForeignKey("UserID")]
	public User User { get; set; }
}
