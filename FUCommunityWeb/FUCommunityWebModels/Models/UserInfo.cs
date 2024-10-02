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
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InfoID { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }

        public DateTime? DOB { get; set; }

        public string Bio { get; set; }

        [MaxLength(255)]
        public string AvatarImage { get; set; }

        // Navigation property
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}
