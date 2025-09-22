using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class createChatTbDataSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Conversation_ConversationId",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Users_UserId",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversation_ConversationId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Message_ParentMessageId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_SenderId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReadStatus_Message_MessageId",
                table: "MessageReadStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Message",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipant",
                table: "ConversationParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation");

            migrationBuilder.RenameTable(
                name: "Message",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "ConversationParticipant",
                newName: "ConversationParticipants");

            migrationBuilder.RenameTable(
                name: "Conversation",
                newName: "conversations");

            migrationBuilder.RenameIndex(
                name: "IX_Message_SenderId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ParentMessageId",
                table: "Messages",
                newName: "IX_Messages_ParentMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ConversationId",
                table: "Messages",
                newName: "IX_Messages_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_UserId",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_conversations",
                table: "conversations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_conversations_ConversationId",
                table: "ConversationParticipants",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReadStatus_Messages_MessageId",
                table: "MessageReadStatus",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ParentMessageId",
                table: "Messages",
                column: "ParentMessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_conversations_ConversationId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReadStatus_Messages_MessageId",
                table: "MessageReadStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ParentMessageId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conversations",
                table: "conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Message");

            migrationBuilder.RenameTable(
                name: "conversations",
                newName: "Conversation");

            migrationBuilder.RenameTable(
                name: "ConversationParticipants",
                newName: "ConversationParticipant");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "Message",
                newName: "IX_Message_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ParentMessageId",
                table: "Message",
                newName: "IX_Message_ParentMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId",
                table: "Message",
                newName: "IX_Message_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_UserId",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Message",
                table: "Message",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipant",
                table: "ConversationParticipant",
                columns: new[] { "ConversationId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Conversation_ConversationId",
                table: "ConversationParticipant",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Users_UserId",
                table: "ConversationParticipant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversation_ConversationId",
                table: "Message",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Message_ParentMessageId",
                table: "Message",
                column: "ParentMessageId",
                principalTable: "Message",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReadStatus_Message_MessageId",
                table: "MessageReadStatus",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
