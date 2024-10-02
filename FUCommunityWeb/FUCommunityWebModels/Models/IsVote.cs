using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.Models
{
    public class IsVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IsVoteID { get; set; }

        public int UserID { get; set; }

        public int PostID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("PostID")]
        public Post Post { get; set; }
    }
}
