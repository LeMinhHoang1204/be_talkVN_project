using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkVN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextChatType",
                table: "TextChats",
                newName: "ConversationType");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Groups",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TextChatPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TextChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextChatPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextChatPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextChatPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextChatPermissions_TextChats_TextChatId",
                        column: x => x.TextChatId,
                        principalTable: "TextChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VoiceChats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MaxQuantity = table.Column<int>(type: "int", nullable: false),
                    CanShareScreen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanRecord = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceChats", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserChatRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ConversationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VoiceChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChatRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChatRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChatRoles_TextChats_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "TextChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChatRoles_VoiceChats_VoiceChatId",
                        column: x => x.VoiceChatId,
                        principalTable: "VoiceChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VoiceChatParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GroupId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VoiceChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceChatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceChatParticipants_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceChatParticipants_VoiceChats_VoiceChatId",
                        column: x => x.VoiceChatId,
                        principalTable: "VoiceChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VoiceChatPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VoiceChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceChatPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceChatPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceChatPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceChatPermissions_VoiceChats_VoiceChatId",
                        column: x => x.VoiceChatId,
                        principalTable: "VoiceChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TextChatPermissions_PermissionId",
                table: "TextChatPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TextChatPermissions_TextChatId",
                table: "TextChatPermissions",
                column: "TextChatId");

            migrationBuilder.CreateIndex(
                name: "IX_TextChatPermissions_UserId",
                table: "TextChatPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoles_ConversationId",
                table: "UserChatRoles",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoles_RoleId",
                table: "UserChatRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoles_UserId",
                table: "UserChatRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoles_VoiceChatId",
                table: "UserChatRoles",
                column: "VoiceChatId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChatParticipants_GroupId",
                table: "VoiceChatParticipants",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChatParticipants_VoiceChatId",
                table: "VoiceChatParticipants",
                column: "VoiceChatId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChatPermissions_PermissionId",
                table: "VoiceChatPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChatPermissions_UserId",
                table: "VoiceChatPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChatPermissions_VoiceChatId",
                table: "VoiceChatPermissions",
                column: "VoiceChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextChatPermissions");

            migrationBuilder.DropTable(
                name: "UserChatRoles");

            migrationBuilder.DropTable(
                name: "VoiceChatParticipants");

            migrationBuilder.DropTable(
                name: "VoiceChatPermissions");

            migrationBuilder.DropTable(
                name: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "ConversationType",
                table: "TextChats",
                newName: "TextChatType");
        }
    }
}
