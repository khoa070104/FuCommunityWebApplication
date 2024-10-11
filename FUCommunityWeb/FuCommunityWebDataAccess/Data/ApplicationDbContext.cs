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
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<IsVote> IsVotes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
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

            // Thiết lập quan hệ giữa ApplicationUser và Point
            modelBuilder.Entity<Point>()
                .HasOne(p => p.User)
                .WithMany(u => u.Points)  // Mối quan hệ 1-nhiều giữa ApplicationUser và Point
                .HasForeignKey(p => p.UserID);

            // Thiết lập mối quan hệ giữa Review và ApplicationUser
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập mối quan hệ giữa Review và Course
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseID)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập chỉ mục duy nhất để mỗi User chỉ có thể đánh giá một Course một lần
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.CourseID, r.UserID })
                .IsUnique()
                .HasDatabaseName("IX_Review_CourseID_UserID");

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Category) // Course có một Category
                .WithMany()  // Category có nhiều Course 
                .HasForeignKey(c => c.CategoryID); // Khóa ngoại

            // Thiết lập quan hệ giữa Deposit và ApplicationUser
            modelBuilder.Entity<Deposit>()
                .HasOne(d => d.User) // Deposit có một User
                .WithMany(u => u.Deposits) // User có nhiều Deposit
                .HasForeignKey(d => d.UserID); // Khóa ngoại

            // Thiết lập quan hệ giữa Post và Document
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Document) // Post có một Document
                .WithMany() // Không cần giữ thông tin Post trong Document
                .HasForeignKey(p => p.DocumentID); // Khóa ngoại
        }
    }
}
