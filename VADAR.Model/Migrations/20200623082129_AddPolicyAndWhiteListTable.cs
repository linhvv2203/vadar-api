// <copyright file="20200623082129_AddPolicyAndWhiteListTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddPolicyAndWhiteListTable.
    /// </summary>
    public partial class AddPolicyAndWhiteListTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Policies",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "WhiteLists",
                table => new
                {
                    Ip = table.Column<string>(maxLength: 150, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    WorkspaceId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiteLists", x => x.Ip);
                    table.ForeignKey(
                        "FK_WhiteLists_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "WorkspacePolicies",
                table => new
                {
                    WorkspaceId = table.Column<int>(nullable: false),
                    PolicyId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspacePolicies", x => new { x.PolicyId, x.WorkspaceId });
                    table.ForeignKey(
                        "FK_WorkspacePolicies_Policies_PolicyId",
                        x => x.PolicyId,
                        "Policies",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_WorkspacePolicies_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_WhiteLists_WorkspaceId",
                "WhiteLists",
                "WorkspaceId");

            migrationBuilder.CreateIndex(
                "IX_WorkspacePolicies_WorkspaceId",
                "WorkspacePolicies",
                "WorkspaceId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "WhiteLists");

            migrationBuilder.DropTable(
                "WorkspacePolicies");

            migrationBuilder.DropTable(
                "Policies");
        }
    }
}
