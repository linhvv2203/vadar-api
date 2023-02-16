// <copyright file="20201007080353_CreateWorkspaceNotificationModel.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// CreateWorkspaceNotificationModel.
    /// </summary>
    public partial class CreateWorkspaceNotificationModel : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "WorkspaceNotifications",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    WorkspaceId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceNotifications", x => x.Id);
                    table.ForeignKey(
                        "FK_WorkspaceNotifications_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_WorkspaceNotifications_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_WorkspaceNotifications_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_WorkspaceNotifications_CreatedById",
                "WorkspaceNotifications",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_WorkspaceNotifications_UpdateById",
                "WorkspaceNotifications",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_WorkspaceNotifications_WorkspaceId",
                "WorkspaceNotifications",
                "WorkspaceId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "WorkspaceNotifications");
        }
    }
}
