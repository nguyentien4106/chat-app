using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_User1Id",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_User2Id",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_User2Id",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_Users",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "Conversations",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "User1Id",
                table: "Conversations",
                newName: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ReceiverId",
                table: "Conversations",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Users",
                table: "Conversations",
                columns: new[] { "SenderId", "ReceiverId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_ReceiverId",
                table: "Conversations",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_SenderId",
                table: "Conversations",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_ReceiverId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_SenderId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ReceiverId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_Users",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Conversations",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Conversations",
                newName: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User2Id",
                table: "Conversations",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Users",
                table: "Conversations",
                columns: new[] { "User1Id", "User2Id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_User1Id",
                table: "Conversations",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_User2Id",
                table: "Conversations",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
