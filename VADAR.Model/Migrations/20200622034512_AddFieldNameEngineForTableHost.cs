// <copyright file="20200622034512_AddFieldNameEngineForTableHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFieldNameEngineForTableHost.
    /// </summary>
    public partial class AddFieldNameEngineForTableHost : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "NameEngine",
                "Hosts",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "NameEngine",
                "Hosts");
        }
    }
}
