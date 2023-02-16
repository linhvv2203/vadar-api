// <copyright file="20200629082453_AddNewFieldsToStoreSecurityAndPerformanceDashboardUrls.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToStoreSecurityAndPerformanceDashboardUrls : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "GrafanaDashboardId",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaDashboardUrl",
                "Workspaces");

            migrationBuilder.AddColumn<long>(
                "GrafanaInventoryDashboardId",
                "Workspaces",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                "GrafanaInventoryDashboardUrl",
                "Workspaces",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                "GrafanaPerformanceDashboardId",
                "Workspaces",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                "GrafanaPerformanceDashboardUrl",
                "Workspaces",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                "GrafanaSecurityDashboardId",
                "Workspaces",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                "GrafanaSecurityDashboardUrl",
                "Workspaces",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "GrafanaInventoryDashboardId",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaInventoryDashboardUrl",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaPerformanceDashboardId",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaPerformanceDashboardUrl",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaSecurityDashboardId",
                "Workspaces");

            migrationBuilder.DropColumn(
                "GrafanaSecurityDashboardUrl",
                "Workspaces");

            migrationBuilder.AddColumn<long>(
                "GrafanaDashboardId",
                "Workspaces",
                "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                "GrafanaDashboardUrl",
                "Workspaces",
                "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
