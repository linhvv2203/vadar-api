// <copyright file="20200619092356_AddGrafanaFieldsForWorkspace.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddGrafanaFieldsForWorkspace : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                "GrafanaDashboardId",
                "Workspaces",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                "GrafanaDashboardUrl",
                "Workspaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "GrafanaFolderUID",
                "Workspaces",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "GrafanaDashboardId",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaDashboardUrl",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaFolderUID",
                "Workspaces");
        }
    }
}
