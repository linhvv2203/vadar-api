// <copyright file="20200613111023_RemoveFieldWorspaceId.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// RemoveFieldWorspaceId.
    /// </summary>
    public partial class RemoveFieldWorspaceId : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "WorkspaceId",
                "Hosts",
                "int",
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
    }
}
