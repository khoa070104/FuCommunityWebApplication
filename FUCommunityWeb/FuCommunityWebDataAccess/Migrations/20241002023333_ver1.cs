using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuCommunityWebDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ver1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseID);
                    table.ForeignKey(
                        name: "FK_Course_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostID);
                    table.ForeignKey(
                        name: "FK_Post_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Question_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    InfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.InfoID);
                    table.ForeignKey(
                        name: "FK_UserInfo_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                columns: table => new
                {
                    LessonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LessonUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lesson", x => x.LessonID);
                    table.ForeignKey(
                        name: "FK_Lesson_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: true),
                    PostID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Document_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID");
                    table.ForeignKey(
                        name: "FK_Document_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "PostID");
                    table.ForeignKey(
                        name: "FK_Document_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IsVote",
                columns: table => new
                {
                    IsVoteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PostID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsVote", x => x.IsVoteID);
                    table.ForeignKey(
                        name: "FK_IsVote_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsVote_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostID = table.Column<int>(type: "int", nullable: true),
                    QuestionID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "PostID");
                    table.ForeignKey(
                        name: "FK_Comment_Question_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Question",
                        principalColumn: "QuestionID");
                    table.ForeignKey(
                        name: "FK_Comment_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    EnrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.EnrollmentID);
                    table.ForeignKey(
                        name: "FK_Enrollment_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollment_Document_DocumentID",
                        column: x => x.DocumentID,
                        principalTable: "Document",
                        principalColumn: "DocumentID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Enrollment_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryID", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Technology", "All about technology" },
                    { 2, "Science", "Science topics" },
                    { 3, "Art", "All about art" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Active", "Email", "LastLogin", "Password", "Points", "RegistrationDate", "Role", "Username" },
                values: new object[,]
                {
                    { 1, true, "john@example.com", new DateTime(2024, 10, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2242), "hashedpassword123", 500, new DateTime(2024, 10, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2241), "Admin", "john_doe" },
                    { 2, true, "jane@example.com", new DateTime(2024, 9, 30, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2257), "hashedpassword456", 300, new DateTime(2024, 9, 22, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2248), "User", "jane_smith" }
                });

            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "CourseID", "CourseImage", "CreatedDate", "Description", "Price", "Status", "Title", "UpdatedDate", "UserID" },
                values: new object[,]
                {
                    { 1, "aspnet_core.png", new DateTime(2024, 9, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2283), "An introductory course to ASP.NET Core.", 49.99m, "Active", "Learn ASP.NET Core", new DateTime(2024, 10, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2284), 1 },
                    { 2, "csharp_master.png", new DateTime(2024, 8, 13, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2288), "Advanced C# programming techniques.", 99.99m, "Active", "Master C# Programming", new DateTime(2024, 9, 22, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2289), 2 }
                });

            migrationBuilder.InsertData(
                table: "Post",
                columns: new[] { "PostID", "CategoryID", "Content", "CreatedDate", "PostImage", "Status", "Title", "UpdatedDate", "UserID" },
                values: new object[,]
                {
                    { 1, 1, "Blazor is a new web framework...", new DateTime(2024, 9, 17, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2314), "blazor_intro.png", "Published", "Introduction to Blazor", new DateTime(2024, 10, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2315), 1 },
                    { 2, 2, "Quantum computing is an emerging field...", new DateTime(2024, 9, 7, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2318), "quantum_basics.png", "Published", "Quantum Computing Basics", new DateTime(2024, 9, 27, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2319), 2 }
                });

            migrationBuilder.InsertData(
                table: "Question",
                columns: new[] { "QuestionID", "Content", "CreatedDate", "Status", "Title", "UpdatedDate", "UserID" },
                values: new object[,]
                {
                    { 1, "Can someone explain dependency injection...", new DateTime(2024, 9, 12, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2342), "Open", "What is Dependency Injection?", new DateTime(2024, 10, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2343), 1 },
                    { 2, "Looking for an explanation on Docker basics...", new DateTime(2024, 9, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2345), "Open", "What are the basics of Docker?", new DateTime(2024, 9, 22, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2346), 2 }
                });

            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "InfoID", "AvatarImage", "Bio", "DOB", "FullName", "Gender", "Phone", "UserID" },
                values: new object[,]
                {
                    { 1, "john_avatar.png", "Software Developer", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John Doe", "M", "123-456-7890", 1 },
                    { 2, "jane_avatar.png", "Data Scientist", new DateTime(1992, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane Smith", "F", "987-654-3210", 2 }
                });

            migrationBuilder.InsertData(
                table: "Document",
                columns: new[] { "DocumentID", "CourseID", "FileUrl", "Name", "PostID", "UploadedAt", "UserID" },
                values: new object[,]
                {
                    { 1, 1, "aspnet_handbook.pdf", "ASP.NET Core Handbook", null, new DateTime(2024, 9, 22, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2463), 1 },
                    { 2, null, "quantum_basics.pdf", "Quantum Basics PDF", 2, new DateTime(2024, 9, 12, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2467), 2 }
                });

            migrationBuilder.InsertData(
                table: "IsVote",
                columns: new[] { "IsVoteID", "PostID", "UserID" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Lesson",
                columns: new[] { "LessonID", "Content", "CourseID", "CreatedDate", "LessonUrl", "Status", "Title", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "In this lesson, you will learn the basics...", 1, new DateTime(2024, 9, 22, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2403), "intro_aspnet_core.mp4", "Completed", "Introduction to ASP.NET Core", new DateTime(2024, 10, 2, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2403) },
                    { 2, "This lesson covers advanced topics...", 2, new DateTime(2024, 8, 23, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2407), "advanced_csharp.mp4", "Completed", "C# Advanced Techniques", new DateTime(2024, 9, 27, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2407) }
                });

            migrationBuilder.InsertData(
                table: "Enrollment",
                columns: new[] { "EnrollmentID", "CourseID", "DocumentID", "EnrollmentDate", "Status", "UserID" },
                values: new object[,]
                {
                    { 1, 2, 1, new DateTime(2024, 9, 27, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2484), "Completed", 1 },
                    { 2, 1, 2, new DateTime(2024, 9, 17, 9, 33, 30, 989, DateTimeKind.Local).AddTicks(2489), "InProgress", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostID",
                table: "Comment",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_QuestionID",
                table: "Comment",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserID",
                table: "Comment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_UserID",
                table: "Course",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CourseID",
                table: "Document",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Document_PostID",
                table: "Document",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Document_UserID",
                table: "Document",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_CourseID",
                table: "Enrollment",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_DocumentID",
                table: "Enrollment",
                column: "DocumentID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_UserID",
                table: "Enrollment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_IsVote_PostID",
                table: "IsVote",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_IsVote_UserID",
                table: "IsVote",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseID",
                table: "Lesson",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_CategoryID",
                table: "Post",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserID",
                table: "Post",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_UserID",
                table: "Question",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_UserID",
                table: "UserInfo",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "IsVote");

            migrationBuilder.DropTable(
                name: "Lesson");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
