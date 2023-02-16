// <copyright file="20200611094031_AddFieldTokenForTableWorkspace.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFieldTokenForTableWorkspace.
    /// </summary>
    public partial class AddFieldTokenForTableWorkspace : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "TokenWorkspace",
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
                "TokenWorkspace",
                "Workspaces");
        }
    }
}
