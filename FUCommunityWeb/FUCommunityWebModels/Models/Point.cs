using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.Models
{
	public class Point
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PointID { get; set; }

		public int? UserID { get; set; }
		public int? PostID { get; set; }

		public int Points { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? DateAward { get; set; } = DateTime.Now;

		// Navigation properties
		[ForeignKey("PostID")]
		public Post Post { get; set; }

		[ForeignKey("UserID")]
		public User User { get; set; }
	}
}
