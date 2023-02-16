// <copyright file="20200611093255_AddFieldWorkspaceForTableHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFieldWorkspaceForTableHost.
    /// </summary>
    public partial class AddFieldWorkspaceForTableHost : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "WorkspaceId",
                "Hosts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                "IX_Hosts_WorkspaceId",
                "Hosts",
                "WorkspaceId");

            migrationBuilder.AddForeignKey(
                "FK_Hosts_Workspaces_WorkspaceId",
                "Hosts",
                "WorkspaceId",
                "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Hosts_Workspaces_WorkspaceId",
                "Hosts");

            migrationBuilder.DropIndex(
                "IX_Hosts_WorkspaceId",
                "Hosts");

            migrationBuilder.DropColumn(
                "WorkspaceId",
                "Hosts");
        }
    }
}
