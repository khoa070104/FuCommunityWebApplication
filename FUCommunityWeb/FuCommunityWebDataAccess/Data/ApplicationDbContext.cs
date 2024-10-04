using Microsoft.EntityFrameworkCore;
using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FuCommunityWebDataAccess.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser> // Sử dụng ApplicationUser thay vì IdentityUser
	{
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<ApplicationUser> Users {  get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Gọi base method để đảm bảo cấu hình mặc định của Identity

            // Thiết lập quan hệ giữa ApplicationUser và Post
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserID);

            // Thiết lập quan hệ giữa ApplicationUser và Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserID);

            // Thiết lập quan hệ giữa ApplicationUser và Vote
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserID);

            // Thiết lập quan hệ giữa ApplicationUser và IsVote
            modelBuilder.Entity<IsVote>()
                .HasOne(iv => iv.User)
                .WithMany(u => u.IsVotes)
                .HasForeignKey(iv => iv.UserID);

            // Thiết lập quan hệ giữa ApplicationUser và Document
            modelBuilder.Entity<Document>()
                .HasOne(d => d.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UserID); 

            // Thiết lập quan hệ giữa ApplicationUser và Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserID);

            // Thiết lập quan hệ giữa Post và Category
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryID);

            // Thiết lập quan hệ giữa Post và Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostID);

            // Thiết lập quan hệ giữa Post và Vote
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Post)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.PostID);

            // Thiết lập quan hệ giữa Course và Document
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Course)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CourseID);

            // Thiết lập quan hệ giữa Course và Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID);

            // Thiết lập quan hệ giữa Question và Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Comments)
                .HasForeignKey(c => c.QuestionID);
        }
    }
}
