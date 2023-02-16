// <copyright file="20200612120709_AddFiedForTableWorkspace.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFiedForTableWorkspace.
    /// </summary>
    public partial class AddFiedForTableWorkspace : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "WazuhRef",
                "Workspaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ZabbixRef",
                "Workspaces",
                nullable: true);
        }

        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "WazuhRef",
                "Workspaces");

            migrationBuilder.DropColumn(
                "ZabbixRef",
                "Workspaces");
        }
    }
}
