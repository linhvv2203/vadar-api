// <copyright file="20200612073703_CreateTableWorkspaceHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// CreateTableWorkspaceHost.
    /// </summary>
    public partial class CreateTableWorkspaceHost : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "WazuhRef",
                "Hosts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ZabbixRef",
                "Hosts",
                nullable: true);

            migrationBuilder.CreateTable(
                "WorkspaceHosts",
                table => new
                {
                    WorkspaceId = table.Column<int>(nullable: false),
                    HostId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceHosts", x => new { x.WorkspaceId, x.HostId });
                    table.ForeignKey(
                        "FK_WorkspaceHosts_Hosts_HostId",
                        x => x.HostId,
                        "Hosts",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_WorkspaceHosts_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_WorkspaceHosts_HostId",
                "WorkspaceHosts",
                "HostId");
        }

        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "WorkspaceHosts");

            migrationBuilder.DropColumn(
                "WazuhRef",
                "Hosts");

            migrationBuilder.DropColumn(
                "ZabbixRef",
                "Hosts");
        }
    }
}
