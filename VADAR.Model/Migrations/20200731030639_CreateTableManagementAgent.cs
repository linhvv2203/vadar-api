// <copyright file="20200731030639_CreateTableManagementAgent.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// CreateTableManagementAgent.
    /// </summary>
    public partial class CreateTableManagementAgent : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AgentOs",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WorkspaceId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentOs", x => x.Id);
                    table.ForeignKey(
                        "FK_AgentOs_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_AgentOs_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_AgentOs_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AgentInstalls",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LinkDownload = table.Column<string>(nullable: true),
                    OsId = table.Column<long>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentInstalls", x => x.Id);
                    table.ForeignKey(
                        "FK_AgentInstalls_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_AgentInstalls_AgentOs_OsId",
                        x => x.OsId,
                        "AgentOs",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AgentInstalls_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_AgentInstalls_CreatedById",
                "AgentInstalls",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_AgentInstalls_OsId",
                "AgentInstalls",
                "OsId");

            migrationBuilder.CreateIndex(
                "IX_AgentInstalls_UpdateById",
                "AgentInstalls",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_AgentOs_CreatedById",
                "AgentOs",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_AgentOs_UpdateById",
                "AgentOs",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_AgentOs_WorkspaceId",
                "AgentOs",
                "WorkspaceId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "AgentInstalls");

            migrationBuilder.DropTable(
                "AgentOs");
        }
    }
}
