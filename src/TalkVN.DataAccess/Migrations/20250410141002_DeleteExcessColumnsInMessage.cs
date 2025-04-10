using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkVN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeleteExcessColumnsInMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "content",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "title",
                table: "Messages");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "VoiceChats",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "NumOfUser",
                table: "TextChats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "VoiceChats");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "VoiceChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "VoiceChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "NumOfUser",
                table: "TextChats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "Messages",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "Messages",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
