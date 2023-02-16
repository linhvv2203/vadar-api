// <copyright file="20200612093745_AddFieldEngineRefGroup.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFieldEngineRefGroup.
    /// </summary>
    public partial class AddFieldEngineRefGroup : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "WazuhRef",
                "Groups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ZabbixRef",
                "Groups",
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
                "Groups");

            migrationBuilder.DropColumn(
                "ZabbixRef",
                "Groups");
        }
    }
}
