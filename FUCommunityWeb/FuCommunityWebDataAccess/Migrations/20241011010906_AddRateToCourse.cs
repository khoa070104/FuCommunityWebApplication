using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuCommunityWebDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRateToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_CourseID",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Review_CourseID_UserID",
                table: "Reviews",
                columns: new[] { "CourseID", "UserID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_CourseID_UserID",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CourseID",
                table: "Reviews",
                column: "CourseID");
        }
    }
}
