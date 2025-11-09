using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzyChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addmembercount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "MemberCount",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedAt_ConversationId",
                table: "Messages",
                columns: new[] { "CreatedAt", "ConversationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedAt_GroupId",
                table: "Messages",
                columns: new[] { "CreatedAt", "GroupId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedAt_ConversationId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedAt_GroupId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MemberCount",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages",
                column: "CreatedAt");
        }
    }
}
