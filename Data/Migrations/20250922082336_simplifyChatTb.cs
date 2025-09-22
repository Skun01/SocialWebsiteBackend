using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class simplifyChatTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ConversationId_CreatedAt",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_ConversationId_SenderId_ClientMessageId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipant_LastReadMessageId",
                table: "ConversationParticipant");

            migrationBuilder.DropIndex(
                name: "IX_Conversation_CreatedAt",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "ClientMessageId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "LastReadMessageId",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Conversation");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Message",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Conversation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Conversation",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId",
                table: "Message",
                column: "ConversationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_ConversationId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Message");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Message",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "ClientMessageId",
                table: "Message",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "ConversationParticipant",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "LastReadMessageId",
                table: "ConversationParticipant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Conversation",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Conversation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Conversation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId_CreatedAt",
                table: "Message",
                columns: new[] { "ConversationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId_SenderId_ClientMessageId",
                table: "Message",
                columns: new[] { "ConversationId", "SenderId", "ClientMessageId" },
                unique: true,
                filter: "[ClientMessageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_LastReadMessageId",
                table: "ConversationParticipant",
                column: "LastReadMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_CreatedAt",
                table: "Conversation",
                column: "CreatedAt");
        }
    }
}
