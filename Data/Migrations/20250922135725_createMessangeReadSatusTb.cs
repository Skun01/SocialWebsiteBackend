using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class createMessangeReadSatusTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentMessageId",
                table: "Message",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "ConversationParticipant",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "ConversationParticipant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LastMessageId",
                table: "Conversation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageTimestamp",
                table: "Conversation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Conversation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MessageReadStatus",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReadStatus", x => new { x.MessageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_MessageReadStatus_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReadStatus_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ParentMessageId",
                table: "Message",
                column: "ParentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadStatus_UserId",
                table: "MessageReadStatus",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Message_ParentMessageId",
                table: "Message",
                column: "ParentMessageId",
                principalTable: "Message",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Message_ParentMessageId",
                table: "Message");

            migrationBuilder.DropTable(
                name: "MessageReadStatus");

            migrationBuilder.DropIndex(
                name: "IX_Message_ParentMessageId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ParentMessageId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "LastMessageTimestamp",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Conversation");
        }
    }
}
