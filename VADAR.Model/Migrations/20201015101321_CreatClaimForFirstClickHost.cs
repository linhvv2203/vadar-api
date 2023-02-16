// <copyright file="20201015101321_CreatClaimForFirstClickHost.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Model.DbInitialize;

namespace VADAR.Model.Migrations
{
    /// <inheritdoc />
    public partial class CreatClaimForFirstClickHost : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dataSeeder = new DataSeeder();
            dataSeeder.ClaimSeedingForFirstClickHost(migrationBuilder);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
