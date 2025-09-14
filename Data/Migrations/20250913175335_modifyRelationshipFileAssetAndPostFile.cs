using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifyRelationshipFileAssetAndPostFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostFile_FileAsset_FileAssetId",
                table: "PostFile");

            migrationBuilder.DropForeignKey(
                name: "FK_PostFile_Posts_PostId",
                table: "PostFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostFile",
                table: "PostFile");

            migrationBuilder.DropIndex(
                name: "IX_PostFile_FileAssetId",
                table: "PostFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileAsset",
                table: "FileAsset");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "PostFile");

            migrationBuilder.RenameTable(
                name: "PostFile",
                newName: "PostFiles");

            migrationBuilder.RenameTable(
                name: "FileAsset",
                newName: "FileAssets");

            migrationBuilder.RenameIndex(
                name: "IX_PostFile_PostId",
                table: "PostFiles",
                newName: "IX_PostFiles_PostId");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostFiles",
                table: "PostFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileAssets",
                table: "FileAssets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostFiles_FileAssetId",
                table: "PostFiles",
                column: "FileAssetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostFiles_FileAssets_FileAssetId",
                table: "PostFiles",
                column: "FileAssetId",
                principalTable: "FileAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostFiles_Posts_PostId",
                table: "PostFiles",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostFiles_FileAssets_FileAssetId",
                table: "PostFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_PostFiles_Posts_PostId",
                table: "PostFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostFiles",
                table: "PostFiles");

            migrationBuilder.DropIndex(
                name: "IX_PostFiles_FileAssetId",
                table: "PostFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileAssets",
                table: "FileAssets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Likes");

            migrationBuilder.RenameTable(
                name: "PostFiles",
                newName: "PostFile");

            migrationBuilder.RenameTable(
                name: "FileAssets",
                newName: "FileAsset");

            migrationBuilder.RenameIndex(
                name: "IX_PostFiles_PostId",
                table: "PostFile",
                newName: "IX_PostFile_PostId");

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "Likes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "PostFile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostFile",
                table: "PostFile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileAsset",
                table: "FileAsset",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostFile_FileAssetId",
                table: "PostFile",
                column: "FileAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostFile_FileAsset_FileAssetId",
                table: "PostFile",
                column: "FileAssetId",
                principalTable: "FileAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostFile_Posts_PostId",
                table: "PostFile",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
