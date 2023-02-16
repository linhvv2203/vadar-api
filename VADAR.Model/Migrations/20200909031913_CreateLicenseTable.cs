// <copyright file="20200909031913_CreateLicenseTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// CreateLicenseTable.
    /// </summary>
    public partial class CreateLicenseTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                "LicenseId",
                "Workspaces",
                nullable: true);

            migrationBuilder.CreateTable(
                "Licenses",
                table => new
                {
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    HostLimit = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                    table.ForeignKey(
                        "FK_Licenses_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Licenses_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Workspaces_LicenseId",
                "Workspaces",
                "LicenseId");

            migrationBuilder.CreateIndex(
                "IX_Licenses_CreatedById",
                "Licenses",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_Licenses_UpdateById",
                "Licenses",
                "UpdateById");

            migrationBuilder.AddForeignKey(
                "FK_Workspaces_Licenses_LicenseId",
                "Workspaces",
                "LicenseId",
                "Licenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Workspaces_Licenses_LicenseId",
                "Workspaces");

            migrationBuilder.DropTable(
                "Licenses");

            migrationBuilder.DropIndex(
                "IX_Workspaces_LicenseId",
                "Workspaces");

            migrationBuilder.DropColumn(
                "LicenseId",
                "Workspaces");
        }
    }
}
