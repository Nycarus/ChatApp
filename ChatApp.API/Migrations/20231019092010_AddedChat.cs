using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatApp.Migrations
{
    public partial class AddedChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Users_UserModelId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_UserModelId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatRoomUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatRoomId = table.Column<int>(type: "integer", nullable: false),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRoomUsers_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatRoomUsers_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomUsers_ChatRoomId",
                table: "ChatRoomUsers",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomUsers_UserProfileId",
                table: "ChatRoomUsers",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Users_UserId",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Users_UserId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "ChatRoomUsers");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles");

            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "UserProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserModelId",
                table: "UserProfiles",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Users_UserModelId",
                table: "UserProfiles",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
