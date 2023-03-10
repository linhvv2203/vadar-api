// <copyright file="20200625103556_UpdateDataPolicies.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Model.DbInitialize;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// UpdateDataPolicies.
    /// </summary>
    public partial class UpdateDataPolicies : Migration
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
