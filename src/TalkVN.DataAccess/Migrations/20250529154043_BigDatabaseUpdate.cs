using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkVN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BigDatabaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupRoles_AspNetUsers_UserId",
                table: "UserGroupRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupRoles_Groups_GroupId",
                table: "UserGroupRoles");

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

            migrationBuilder.DropIndex(
                name: "IX_UserGroupRoles_UserId",
                table: "UserGroupRoles");

            migrationBuilder.DropColumn(
                name: "AcceptedBy",
                table: "UserGroupRoles");

            migrationBuilder.DropColumn(
                name: "InvitedBy",
                table: "UserGroupRoles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserGroupRoles");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "UserGroupRoles",
                newName: "UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupRoles_GroupId",
                table: "UserGroupRoles",
                newName: "IX_UserGroupRoles_UserGroupId");

            migrationBuilder.CreateTable(
                name: "OverridePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_OverridePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OverridePermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverridePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverridePermissions_TextChats_TextChatId",
                        column: x => x.TextChatId,
                        principalTable: "TextChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AcceptedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvitedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroup_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroup_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OverridePermissions_PermissionId",
                table: "OverridePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_OverridePermissions_TextChatId",
                table: "OverridePermissions",
                column: "TextChatId");

            migrationBuilder.CreateIndex(
                name: "IX_OverridePermissions_UserId",
                table: "OverridePermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_GroupId",
                table: "UserGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_UserId",
                table: "UserGroup",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupRoles_UserGroup_UserGroupId",
                table: "UserGroupRoles",
                column: "UserGroupId",
                principalTable: "UserGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupRoles_UserGroup_UserGroupId",
                table: "UserGroupRoles");

            migrationBuilder.DropTable(
                name: "OverridePermissions");

            migrationBuilder.DropTable(
                name: "UserGroup");

            migrationBuilder.RenameColumn(
                name: "UserGroupId",
                table: "UserGroupRoles",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupRoles_UserGroupId",
                table: "UserGroupRoles",
                newName: "IX_UserGroupRoles_GroupId");

            migrationBuilder.AddColumn<string>(
                name: "AcceptedBy",
                table: "UserGroupRoles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InvitedBy",
                table: "UserGroupRoles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserGroupRoles",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TextChatPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TextChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                    CanRecord = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanShareScreen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MaxQuantity = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextChatId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VoiceChatId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                        name: "FK_UserChatRoles_TextChats_TextChatId",
                        column: x => x.TextChatId,
                        principalTable: "TextChats",
                        principalColumn: "Id");
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
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
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
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VoiceChatId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                name: "IX_UserGroupRoles_UserId",
                table: "UserGroupRoles",
                column: "UserId");

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
                name: "IX_UserChatRoles_RoleId",
                table: "UserChatRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoles_TextChatId",
                table: "UserChatRoles",
                column: "TextChatId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupRoles_AspNetUsers_UserId",
                table: "UserGroupRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupRoles_Groups_GroupId",
                table: "UserGroupRoles",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
