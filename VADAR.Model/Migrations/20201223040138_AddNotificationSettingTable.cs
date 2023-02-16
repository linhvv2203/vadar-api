// <copyright file="20201223040138_AddNotificationSettingTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddNotificationSettingTable.
    /// </summary>
    public partial class AddNotificationSettingTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Activate = table.Column<bool>(nullable: false),
                    WorkspaceId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettingConditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Condition = table.Column<string>(maxLength: 100, nullable: true),
                    Value = table.Column<int>(nullable: false),
                    NotificationType = table.Column<int>(nullable: false),
                    NotificationSettingId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettingConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettingConditions_NotificationSettings_Notificat~",
                        column: x => x.NotificationSettingId,
                        principalTable: "NotificationSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettingConditions_NotificationSettingId",
                table: "NotificationSettingConditions",
                column: "NotificationSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_WorkspaceId",
                table: "NotificationSettings",
                column: "WorkspaceId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSettingConditions");

            migrationBuilder.DropTable(
                name: "NotificationSettings");
        }
    }
}
