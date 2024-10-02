using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.Models
{
	public class Enrollment
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }

        public int UserID { get; set; }

        public int CourseID { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; }

        public int? DocumentID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }

        [ForeignKey("DocumentID")]
        public virtual Document Document { get; set; }
    }
}
