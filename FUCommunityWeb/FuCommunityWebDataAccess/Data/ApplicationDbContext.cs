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

            // User and UserInfo relationship
        modelBuilder.Entity<UserInfo>()
                .HasOne(ui => ui.User) // Reference to the User navigation property
                .WithOne(u => u.UserInfo) // Reference to the UserInfo navigation property
                .HasForeignKey<UserInfo>(ui => ui.UserID); // Foreign key in UserInfo
        

        // User and Post relationship
        modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserID);

            // Post and Category relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryID);

            // Course and Lesson relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseID);

            // User and Enrollment relationship
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.Cascade based on your needs

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID)
                .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.Cascade

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Document)
                .WithMany(d => d.Enrollments)
                .HasForeignKey(e => e.DocumentID)
                .OnDelete(DeleteBehavior.SetNull);

            // User and Question relationship
            modelBuilder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserID);

            // Question and Comment relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Comments)
                .HasForeignKey(c => c.QuestionID)
                .IsRequired(false);

            // Post and Comment relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostID)
                .IsRequired(false);

            // User and Vote relationship
            modelBuilder.Entity<IsVote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserID);

            modelBuilder.Entity<IsVote>()
                .HasOne(v => v.Post)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.PostID);

            // Document and User relationship
            modelBuilder.Entity<Document>()
                .HasOne(d => d.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UserID);

            // Document and Course relationship
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Course)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CourseID)
                .IsRequired(false);

            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)"); // Set precision and scale here
                                                 // IsVote configuration with no cascade delete
            modelBuilder.Entity<IsVote>()
                .HasOne(iv => iv.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(iv => iv.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Change cascade to restrict

            modelBuilder.Entity<IsVote>()
                .HasOne(iv => iv.Post)
                .WithMany(p => p.Votes)
                .HasForeignKey(iv => iv.PostID)
                .OnDelete(DeleteBehavior.Restrict); // Change cascade to restrict

            Seed(modelBuilder);
        }
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Category data
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, CategoryName = "Technology", Description = "All about technology" },
                new Category { CategoryID = 2, CategoryName = "Science", Description = "Science topics" },
                new Category { CategoryID = 3, CategoryName = "Art", Description = "All about art" }
            );

            // Seed User data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    Username = "john_doe",
                    Password = "hashedpassword123",
                    Email = "john@example.com",
                    Role = "Admin",
                    Active = true,
                    Points = 500,
                    RegistrationDate = DateTime.Now,
                    LastLogin = DateTime.Now
                },
                new User
                {
                    UserID = 2,
                    Username = "jane_smith",
                    Password = "hashedpassword456",
                    Email = "jane@example.com",
                    Role = "User",
                    Active = true,
                    Points = 300,
                    RegistrationDate = DateTime.Now.AddDays(-10),
                    LastLogin = DateTime.Now.AddDays(-2)
                }
            );

            // Seed Course data
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseID = 1,
                    UserID = 1,
                    Title = "Learn ASP.NET Core",
                    Description = "An introductory course to ASP.NET Core.",
                    Price = 49.99m,
                    CreatedDate = DateTime.Now.AddDays(-30),
                    UpdatedDate = DateTime.Now,
                    CourseImage = "aspnet_core.png",
                    Status = "Active"
                },
                new Course
                {
                    CourseID = 2,
                    UserID = 2,
                    Title = "Master C# Programming",
                    Description = "Advanced C# programming techniques.",
                    Price = 99.99m,
                    CreatedDate = DateTime.Now.AddDays(-50),
                    UpdatedDate = DateTime.Now.AddDays(-10),
                    CourseImage = "csharp_master.png",
                    Status = "Active"
                }
            );

            // Seed Post data
            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    PostID = 1,
                    UserID = 1,
                    CategoryID = 1,
                    Title = "Introduction to Blazor",
                    Content = "Blazor is a new web framework...",
                    PostImage = "blazor_intro.png",
                    CreatedDate = DateTime.Now.AddDays(-15),
                    UpdatedDate = DateTime.Now,
                    Status = "Published"
                },
                new Post
                {
                    PostID = 2,
                    UserID = 2,
                    CategoryID = 2,
                    Title = "Quantum Computing Basics",
                    Content = "Quantum computing is an emerging field...",
                    PostImage = "quantum_basics.png",
                    CreatedDate = DateTime.Now.AddDays(-25),
                    UpdatedDate = DateTime.Now.AddDays(-5),
                    Status = "Published"
                }
            );

            // Seed Question data
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    QuestionID = 1,
                    UserID = 1,
                    Title = "What is Dependency Injection?",
                    Content = "Can someone explain dependency injection...",
                    CreatedDate = DateTime.Now.AddDays(-20),
                    UpdatedDate = DateTime.Now,
                    Status = "Open"
                },
                new Question
                {
                    QuestionID = 2,
                    UserID = 2,
                    Title = "What are the basics of Docker?",
                    Content = "Looking for an explanation on Docker basics...",
                    CreatedDate = DateTime.Now.AddDays(-30),
                    UpdatedDate = DateTime.Now.AddDays(-10),
                    Status = "Open"
                }
            );

            // Seed UserInfo data
            modelBuilder.Entity<UserInfo>().HasData(
                new UserInfo
                {
                    InfoID = 1,
                    FullName = "John Doe",
                    Phone = "123-456-7890",
                    Gender = "M",
                    DOB = new DateTime(1990, 1, 1),
                    Bio = "Software Developer",
                    AvatarImage = "john_avatar.png",
                    UserID = 1
                },
                new UserInfo
                {
                    InfoID = 2,
                    FullName = "Jane Smith",
                    Phone = "987-654-3210",
                    Gender = "F",
                    DOB = new DateTime(1992, 5, 15),
                    Bio = "Data Scientist",
                    AvatarImage = "jane_avatar.png",
                    UserID = 2
                }
            );

            // Seed Lesson data
            modelBuilder.Entity<Lesson>().HasData(
                new Lesson
                {
                    LessonID = 1,
                    CourseID = 1,
                    Title = "Introduction to ASP.NET Core",
                    Content = "In this lesson, you will learn the basics...",
                    CreatedDate = DateTime.Now.AddDays(-10),
                    UpdatedDate = DateTime.Now,
                    LessonUrl = "intro_aspnet_core.mp4",
                    Status = "Completed"
                },
                new Lesson
                {
                    LessonID = 2,
                    CourseID = 2,
                    Title = "C# Advanced Techniques",
                    Content = "This lesson covers advanced topics...",
                    CreatedDate = DateTime.Now.AddDays(-40),
                    UpdatedDate = DateTime.Now.AddDays(-5),
                    LessonUrl = "advanced_csharp.mp4",
                    Status = "Completed"
                }
            );

            // Seed Document data
            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    DocumentID = 1,
                    UserID = 1,
                    CourseID = 1,
                    PostID = null,
                    Name = "ASP.NET Core Handbook",
                    FileUrl = "aspnet_handbook.pdf",
                    UploadedAt = DateTime.Now.AddDays(-10)
                },
                new Document
                {
                    DocumentID = 2,
                    UserID = 2,
                    PostID = 2,
                    CourseID = null,
                    Name = "Quantum Basics PDF",
                    FileUrl = "quantum_basics.pdf",
                    UploadedAt = DateTime.Now.AddDays(-20)
                }
            );

            // Seed Enrollment data
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment
                {
                    EnrollmentID = 1,
                    UserID = 1,
                    CourseID = 2,
                    EnrollmentDate = DateTime.Now.AddDays(-5),
                    Status = "Completed",
                    DocumentID = 1
                },
                new Enrollment
                {
                    EnrollmentID = 2,
                    UserID = 2,
                    CourseID = 1,
                    EnrollmentDate = DateTime.Now.AddDays(-15),
                    Status = "InProgress",
                    DocumentID = 2
                }
            );

            // Seed IsVote data
            modelBuilder.Entity<IsVote>().HasData(
                new IsVote
                {
                    IsVoteID = 1,
                    UserID = 1,
                    PostID = 1
                },
                new IsVote
                {
                    IsVoteID = 2,
                    UserID = 2,
                    PostID = 2
                }
            );
        }
    }
}
