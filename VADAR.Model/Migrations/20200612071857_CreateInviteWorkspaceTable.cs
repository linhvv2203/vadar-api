// <copyright file="20200612071857_CreateInviteWorkspaceTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// CreateInviteWorkspaceTable.
    /// </summary>
    public partial class CreateInviteWorkspaceTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "InviteWorkspaceRoles",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WorkspaceRoleId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    GroupBy = table.Column<string>(nullable: true),
                    ExpriredDate = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteWorkspaceRoles", x => x.Id);
                    table.ForeignKey(
                        "FK_InviteWorkspaceRoles_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_InviteWorkspaceRoles_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_InviteWorkspaceRoles_WorkspaceRoles_WorkspaceRoleId",
                        x => x.WorkspaceRoleId,
                        "WorkspaceRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_InviteWorkspaceRoles_CreatedById",
                "InviteWorkspaceRoles",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_InviteWorkspaceRoles_UserId",
                "InviteWorkspaceRoles",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_InviteWorkspaceRoles_WorkspaceRoleId",
                "InviteWorkspaceRoles",
                "WorkspaceRoleId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "InviteWorkspaceRoles");
        }
    }
}
