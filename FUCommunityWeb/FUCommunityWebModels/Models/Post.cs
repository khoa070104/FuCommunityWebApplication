using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public int UserID { get; set; }

        public int CategoryID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        [StringLength(255)]
        public string PostImage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<IsVote> Votes { get; set; }
    }
}
