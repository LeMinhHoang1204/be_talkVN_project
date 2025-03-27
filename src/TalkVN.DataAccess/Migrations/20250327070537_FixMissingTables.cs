using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkVN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentNotifications_AspNetUsers_ReceiverUserId",
                table: "CommentNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Groups_GroupId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_PostNotifications_AspNetUsers_ReceiverUserId",
                table: "PostNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PostNotifications_ReceiverUserId",
                table: "PostNotifications");

            migrationBuilder.DropIndex(
                name: "IX_CommentNotifications_ReceiverUserId",
                table: "CommentNotifications");

            migrationBuilder.DropColumn(
                name: "ReceiverUserId",
                table: "PostNotifications");

            migrationBuilder.DropColumn(
                name: "ReceiverUserId",
                table: "CommentNotifications");

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

            migrationBuilder.CreateTable(
                name: "GroupNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GroupId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reference = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastInteractorUserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupNotifications_AspNetUsers_LastInteractorUserId",
                        column: x => x.LastInteractorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupNotifications_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationReceivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GroupNotificationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReceiverId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_NotificationReceivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationReceivers_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationReceivers_GroupNotifications_GroupNotificationId",
                        column: x => x.GroupNotificationId,
                        principalTable: "GroupNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GroupNotifications_GroupId",
                table: "GroupNotifications",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupNotifications_LastInteractorUserId",
                table: "GroupNotifications",
                column: "LastInteractorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_GroupNotificationId",
                table: "NotificationReceivers",
                column: "GroupNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_ReceiverId",
                table: "NotificationReceivers",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Groups_GroupId",
                table: "Conversations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Groups_GroupId",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "NotificationReceivers");

            migrationBuilder.DropTable(
                name: "GroupNotifications");

            migrationBuilder.DropColumn(
                name: "content",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "title",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserId",
                table: "PostNotifications",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserId",
                table: "CommentNotifications",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PostNotifications_ReceiverUserId",
                table: "PostNotifications",
                column: "ReceiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentNotifications_ReceiverUserId",
                table: "CommentNotifications",
                column: "ReceiverUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentNotifications_AspNetUsers_ReceiverUserId",
                table: "CommentNotifications",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Groups_GroupId",
                table: "Conversations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostNotifications_AspNetUsers_ReceiverUserId",
                table: "PostNotifications",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
