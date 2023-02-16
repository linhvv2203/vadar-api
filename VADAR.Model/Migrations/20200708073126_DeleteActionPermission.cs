// <copyright file="20200708073126_DeleteActionPermission.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Model.DbInitialize;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// DeleteActionPermission.
    /// </summary>
    public partial class DeleteActionPermission : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dataSeeder = new DataSeeder();
            dataSeeder.DeletePermissionsForActionModule(migrationBuilder);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
