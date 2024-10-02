using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace FuCommunityWebModels.Models
{
	public class Question
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int QuestionID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
