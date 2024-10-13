using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.ViewModels
{
    public class EditCourseVM
    {
        public int CourseID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string? CourseImage { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [NotMapped]
        [Display(Name = "Course Image")]
        public IFormFile? CourseImageFile { get; set; }
    }
}