// <copyright file="20200611075727_AddFieldForTableHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFieldForTableHost.
    /// </summary>
    public partial class AddFieldForTableHost : Migration
    {
        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Name",
                "GroupHosts");

            migrationBuilder.AddColumn<string>(
                "Os",
                "Hosts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                "Status",
                "Hosts",
                nullable: false,
                defaultValue: 0);
        }

        /// <summary>
        /// Excute Migration.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Os",
                "Hosts");

            migrationBuilder.DropColumn(
                "Status",
                "Hosts");

            migrationBuilder.AddColumn<string>(
                "Name",
                "GroupHosts",
                "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
