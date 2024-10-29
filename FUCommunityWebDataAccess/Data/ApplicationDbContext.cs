using Microsoft.EntityFrameworkCore;
using FUCommunityWebDataAccess.Models;

namespace FUCommunityWebDataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing relationships configuration...
            
            // Seed Categories data
            modelBuilder.Entity<Category>().HasData(
                new Category 
                { 
                    CategoryID = 1,
                    CategoryName = "Programming",
                    Description = "All about programming languages and software development."
                },
                new Category 
                { 
                    CategoryID = 2,
                    CategoryName = "Web Development",
                    Description = "Resources and articles on web development technologies and frameworks."
                },
                new Category 
                { 
                    CategoryID = 3,
                    CategoryName = "Data Science",
                    Description = "Insights and techniques in data analysis, machine learning, and statistics."
                },
                new Category 
                { 
                    CategoryID = 4,
                    CategoryName = "Mobile Development",
                    Description = "Content focused on mobile app development for Android and iOS."
                }
            );

            // Rest of your existing OnModelCreating code...
        }
    }
} 