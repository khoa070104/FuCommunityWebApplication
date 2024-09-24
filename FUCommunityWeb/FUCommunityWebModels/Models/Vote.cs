using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.Models
{
	public class Vote
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int VoteID { get; set; }

		public int? UserID { get; set; }
		public int? PostID { get; set; }

		[MaxLength(10)]
		public string VoteType { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? VotedAt { get; set; } = DateTime.Now;

		// Navigation properties
		[ForeignKey("UserID")]
		public User User { get; set; }

		[ForeignKey("PostID")]
		public Post Post { get; set; }
	}
}
