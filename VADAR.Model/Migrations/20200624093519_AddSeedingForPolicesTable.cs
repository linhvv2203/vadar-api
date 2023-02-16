// <copyright file="20200624093519_AddSeedingForPolicesTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Model.DbInitialize;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddSeedingForPolicesTable.
    /// </summary>
    public partial class AddSeedingForPolicesTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dataSeeder = new DataSeeder();
            dataSeeder.PolicySeeding(migrationBuilder);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
