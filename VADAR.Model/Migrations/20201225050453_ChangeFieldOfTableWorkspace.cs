// <copyright file="20201225050453_ChangeFieldOfTableWorkspace.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFieldOfTableWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreatedOrganisation",
                table: "Workspaces");

            migrationBuilder.AddColumn<string>(
                name: "OrgId",
                table: "Workspaces",
                nullable: true,
                defaultValue: string.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreatedOrganisation",
                table: "Workspaces");
        }
    }
}
