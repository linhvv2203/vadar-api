// <copyright file="20200616044107_AddFielsInviteToInInviteWorkspaceRoleTable.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace VADAR.Model.Migrations
{
    /// <summary>
    /// AddFielsInviteToInInviteWorkspaceRoleTable.
    /// </summary>
    public partial class AddFielsInviteToInInviteWorkspaceRoleTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "InviteTo",
                "InviteWorkspaceRoles",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "InviteTo",
                "InviteWorkspaceRoles");
        }
    }
}
