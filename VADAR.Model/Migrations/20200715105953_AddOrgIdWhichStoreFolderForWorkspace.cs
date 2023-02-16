// <copyright file="20200715105953_AddOrgIdWhichStoreFolderForWorkspace.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgIdWhichStoreFolderForWorkspace : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                "GrafanaOrgId",
                "Workspaces",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "GrafanaOrgId",
                "Workspaces");
        }
    }
}
