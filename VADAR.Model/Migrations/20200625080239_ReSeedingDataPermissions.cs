// <copyright file="20200625080239_ReSeedingDataPermissions.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Model.DbInitialize;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// ReSeedingDataPermissions.
    /// </summary>
    public partial class ReSeedingDataPermissions : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dataSeeder = new DataSeeder();
            dataSeeder.PermissionsSeeding(migrationBuilder);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
