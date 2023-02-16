// <copyright file="20200626094540_ReviseKeyForWhiteListTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <inheritdoc />
    public partial class ReviseKeyForWhiteListTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "WhiteLists");

            migrationBuilder.CreateTable(
                "WhiteIps",
                table => new
                {
                    Ip = table.Column<string>(maxLength: 150, nullable: false),
                    WorkspaceId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiteIps", x => new { x.Ip, x.WorkspaceId });
                    table.ForeignKey(
                        "FK_WhiteIps_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_WhiteIps_WorkspaceId",
                "WhiteIps",
                "WorkspaceId");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "WhiteIps");

            migrationBuilder.CreateTable(
                "WhiteLists",
                table => new
                {
                    Ip = table.Column<string>("varchar(150) CHARACTER SET utf8mb4", maxLength: 150, nullable: false),
                    Description = table.Column<string>("longtext CHARACTER SET utf8mb4", nullable: true),
                    WorkspaceId = table.Column<int>("int", nullable: false),
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

            migrationBuilder.CreateIndex(
                "IX_WhiteLists_WorkspaceId",
                "WhiteLists",
                "WorkspaceId");
        }
    }
}
