using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class editIndexForLikeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_TargetId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_TargetId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_TargetId_Type",
                table: "Likes",
                columns: new[] { "UserId", "TargetId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_TargetId_Type",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_TargetId",
                table: "Likes",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_TargetId",
                table: "Likes",
                columns: new[] { "UserId", "TargetId" },
                unique: true);
        }
    }
}
