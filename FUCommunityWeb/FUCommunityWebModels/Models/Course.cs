﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace FuCommunityWebModels.Models
{
	public class Course
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseID { get; set; }

        public string UserID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(255)]
        public string CourseImage { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}