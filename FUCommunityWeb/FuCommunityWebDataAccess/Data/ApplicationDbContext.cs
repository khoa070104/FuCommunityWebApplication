using Microsoft.EntityFrameworkCore;
using FuCommunityWebModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebDataAccess.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Post>()
				.HasOne(p => p.User)
				.WithMany(u => u.Posts)
				.HasForeignKey(p => p.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Post>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Posts)
				.HasForeignKey(p => p.CategoryID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Course>()
				.HasOne(c => c.User)
				.WithMany(u => u.Courses)
				.HasForeignKey(c => c.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Lesson>()
				.HasOne(l => l.Course)
				.WithMany(c => c.Lessons)
				.HasForeignKey(l => l.CourseID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Question>()
				.HasOne(q => q.User)
				.WithMany(u => u.Questions)
				.HasForeignKey(q => q.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Comment>()
				.HasOne(cm => cm.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(cm => cm.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Comment>()
				.HasOne(cm => cm.Post)
				.WithMany(p => p.Comments)
				.HasForeignKey(cm => cm.PostID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Comment>()
				.HasOne(cm => cm.Question)
				.WithMany(q => q.Comments)
				.HasForeignKey(cm => cm.QuestionID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Document>()
				.HasOne(d => d.User)
				.WithMany(u => u.Documents)
				.HasForeignKey(d => d.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Document>()
				.HasOne(d => d.Course)
				.WithMany(c => c.Documents)
				.HasForeignKey(d => d.CourseID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Document>()
				.HasOne(d => d.Post)
				.WithMany(p => p.Documents)
				.HasForeignKey(d => d.PostID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Vote>()
				.HasOne(v => v.User)
				.WithMany(u => u.Votes)
				.HasForeignKey(v => v.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Vote>()
				.HasOne(v => v.Post)
				.WithMany(p => p.Votes)
				.HasForeignKey(v => v.PostID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Point>()
				.HasOne(p => p.User)
				.WithMany(u => u.PointsEarned)
				.HasForeignKey(p => p.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Point>()
				.HasOne(p => p.Post)
				.WithMany(p => p.Points)
				.HasForeignKey(p => p.PostID)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
