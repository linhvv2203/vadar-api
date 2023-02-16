// <copyright file="20200610104220_DBInitialize.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Model.DbInitialize;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// Database Initialize Migration.
    /// </summary>
    public partial class DBInitialize : Migration
    {
        /// <summary>
        /// Execute Migration.
        /// </summary>
        /// <param name="migrationBuilder">Migration Builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Countries",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Languages",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Flag = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Permissions",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PermissionType = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<string>(maxLength: 150, nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    IsProfileUpdated = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: false),
                    ApprovedById = table.Column<string>(maxLength: 150, nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    ApproverComment = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        "FK_Users_Users_ApprovedById",
                        x => x.ApprovedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Users_Countries_CountryId",
                        x => x.CountryId,
                        "Countries",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Claims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        "FK_Claims_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Claims_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Hosts",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.Id);
                    table.ForeignKey(
                        "FK_Hosts_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Hosts_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Roles",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        "FK_Roles_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Roles_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "UserLanguages",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLanguages", x => new { x.LanguageId, x.UserId });
                    table.ForeignKey(
                        "FK_UserLanguages_Languages_LanguageId",
                        x => x.LanguageId,
                        "Languages",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_UserLanguages_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Workspaces",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        "FK_Workspaces_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Workspaces_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "UserClaims",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    ClaimId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => new { x.ClaimId, x.UserId });
                    table.ForeignKey(
                        "FK_UserClaims_Claims_ClaimId",
                        x => x.ClaimId,
                        "Claims",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_UserClaims_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "RolePermissions",
                table => new
                {
                    PermissionId = table.Column<long>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        "FK_RolePermissions_Permissions_PermissionId",
                        x => x.PermissionId,
                        "Permissions",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_RolePermissions_Roles_RoleId",
                        x => x.RoleId,
                        "Roles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "RoleUsers",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUsers", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        "FK_RoleUsers_Roles_RoleId",
                        x => x.RoleId,
                        "Roles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_RoleUsers_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Groups",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WorkspaceId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        "FK_Groups_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Groups_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Groups_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "WorkspaceRoles",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdateById = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WorkspaceId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceRoles", x => x.Id);
                    table.ForeignKey(
                        "FK_WorkspaceRoles_Users_CreatedById",
                        x => x.CreatedById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_WorkspaceRoles_Users_UpdateById",
                        x => x.UpdateById,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_WorkspaceRoles_Workspaces_WorkspaceId",
                        x => x.WorkspaceId,
                        "Workspaces",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "GroupHosts",
                table => new
                {
                    HostId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupHosts", x => new { x.HostId, x.GroupId });
                    table.ForeignKey(
                        "FK_GroupHosts_Groups_GroupId",
                        x => x.GroupId,
                        "Groups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_GroupHosts_Hosts_HostId",
                        x => x.HostId,
                        "Hosts",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "WorkspaceRolePermissions",
                table => new
                {
                    PermissionId = table.Column<long>(nullable: false),
                    WorkspaceRoleId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceRolePermissions", x => new { x.WorkspaceRoleId, x.PermissionId });
                    table.ForeignKey(
                        "FK_WorkspaceRolePermissions_Permissions_PermissionId",
                        x => x.PermissionId,
                        "Permissions",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_WorkspaceRolePermissions_WorkspaceRoles_WorkspaceRoleId",
                        x => x.WorkspaceRoleId,
                        "WorkspaceRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "WorkspaceRoleUsers",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    WorkspaceRoleId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceRoleUsers", x => new { x.WorkspaceRoleId, x.UserId });
                    table.ForeignKey(
                        "FK_WorkspaceRoleUsers_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_WorkspaceRoleUsers_WorkspaceRoles_WorkspaceRoleId",
                        x => x.WorkspaceRoleId,
                        "WorkspaceRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Claims_CreatedById",
                "Claims",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_Claims_UpdateById",
                "Claims",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_GroupHosts_GroupId",
                "GroupHosts",
                "GroupId");

            migrationBuilder.CreateIndex(
                "IX_Groups_CreatedById",
                "Groups",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_Groups_UpdateById",
                "Groups",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_Groups_WorkspaceId",
                "Groups",
                "WorkspaceId");

            migrationBuilder.CreateIndex(
                "IX_Hosts_CreatedById",
                "Hosts",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_Hosts_UpdateById",
                "Hosts",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_RolePermissions_PermissionId",
                "RolePermissions",
                "PermissionId");

            migrationBuilder.CreateIndex(
                "IX_Roles_CreatedById",
                "Roles",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_Roles_UpdateById",
                "Roles",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_RoleUsers_UserId",
                "RoleUsers",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_UserClaims_UserId",
                "UserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_UserLanguages_UserId",
                "UserLanguages",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_Users_ApprovedById",
                "Users",
                "ApprovedById");

            migrationBuilder.CreateIndex(
                "IX_Users_CountryId",
                "Users",
                "CountryId");

            migrationBuilder.CreateIndex(
                "IX_Users_Email",
                "Users",
                "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Users_UserName",
                "Users",
                "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_WorkspaceRolePermissions_PermissionId",
                "WorkspaceRolePermissions",
                "PermissionId");

            migrationBuilder.CreateIndex(
                "IX_WorkspaceRoles_CreatedById",
                "WorkspaceRoles",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_WorkspaceRoles_UpdateById",
                "WorkspaceRoles",
                "UpdateById");

            migrationBuilder.CreateIndex(
                "IX_WorkspaceRoles_WorkspaceId",
                "WorkspaceRoles",
                "WorkspaceId");

            migrationBuilder.CreateIndex(
                "IX_WorkspaceRoleUsers_UserId",
                "WorkspaceRoleUsers",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_Workspaces_CreatedById",
                "Workspaces",
                "CreatedById");

            migrationBuilder.CreateIndex(
                "IX_Workspaces_UpdateById",
                "Workspaces",
                "UpdateById");

            var dataSeeder = new DataSeeder();
            dataSeeder.SeedingData(migrationBuilder);
        }

        /// <summary>
        /// Rollback the migration.
        /// </summary>
        /// <param name="migrationBuilder">Migration Builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "GroupHosts");

            migrationBuilder.DropTable(
                "RolePermissions");

            migrationBuilder.DropTable(
                "RoleUsers");

            migrationBuilder.DropTable(
                "UserClaims");

            migrationBuilder.DropTable(
                "UserLanguages");

            migrationBuilder.DropTable(
                "WorkspaceRolePermissions");

            migrationBuilder.DropTable(
                "WorkspaceRoleUsers");

            migrationBuilder.DropTable(
                "Groups");

            migrationBuilder.DropTable(
                "Hosts");

            migrationBuilder.DropTable(
                "Roles");

            migrationBuilder.DropTable(
                "Claims");

            migrationBuilder.DropTable(
                "Languages");

            migrationBuilder.DropTable(
                "Permissions");

            migrationBuilder.DropTable(
                "WorkspaceRoles");

            migrationBuilder.DropTable(
                "Workspaces");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Countries");
        }
    }
}
