using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuCommunityWebModels.Models
{
	public class Category
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CategoryID { get; set; }

		[Required]
		[MaxLength(100)]
		public string CategoryName { get; set; }

		public string Description { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? CreatedDate { get; set; } = DateTime.Now;

		[DataType(DataType.DateTime)]
		public DateTime? UpdatedDate { get; set; }

		// Navigation property
		public ICollection<Post> Posts { get; set; }
	}
}
