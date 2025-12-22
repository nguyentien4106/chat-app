using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzyChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuickMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PinMessages_MessageId_ConversationId",
                table: "PinMessages");

            migrationBuilder.DropIndex(
                name: "IX_PinMessages_MessageId_GroupId",
                table: "PinMessages");

            migrationBuilder.CreateTable(
                name: "QuickMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuickMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PinMessages_MessageId_ConversationId",
                table: "PinMessages",
                columns: new[] { "MessageId", "ConversationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PinMessages_MessageId_GroupId",
                table: "PinMessages",
                columns: new[] { "MessageId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuickMessages_Key_UserId",
                table: "QuickMessages",
                columns: new[] { "Key", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuickMessages_UserId",
                table: "QuickMessages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuickMessages");

            migrationBuilder.DropIndex(
                name: "IX_PinMessages_MessageId_ConversationId",
                table: "PinMessages");

            migrationBuilder.DropIndex(
                name: "IX_PinMessages_MessageId_GroupId",
                table: "PinMessages");

            migrationBuilder.CreateIndex(
                name: "IX_PinMessages_MessageId_ConversationId",
                table: "PinMessages",
                columns: new[] { "MessageId", "ConversationId" },
                unique: true,
                filter: "\"ConversationId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PinMessages_MessageId_GroupId",
                table: "PinMessages",
                columns: new[] { "MessageId", "GroupId" },
                unique: true,
                filter: "\"GroupId\" IS NOT NULL");
        }
    }
}
