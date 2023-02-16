﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VADAR.Model;

namespace VADAR.Model.Migrations
{
    [DbContext(typeof(VADARDbContext))]
    [Migration("20200623082129_AddPolicyAndWhiteListTable")]
    partial class AddPolicyAndWhiteListTable
    {
        /// <inheritdoc/>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("VADAR.Model.Models.Claim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UpdateById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("VADAR.Model.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("DisplayName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("VADAR.Model.Models.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UpdateById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("WazuhRef")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.Property<string>("ZabbixRef")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("VADAR.Model.Models.GroupHost", b =>
                {
                    b.Property<Guid>("HostId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("char(36)");

                    b.HasKey("HostId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupHosts");
                });

            modelBuilder.Entity("VADAR.Model.Models.Host", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("MachineId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("NameEngine")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Os")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UpdateById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("WazuhRef")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ZabbixRef")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("Hosts");
                });

            modelBuilder.Entity("VADAR.Model.Models.InviteWorkspaceRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ExpriredDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GroupBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("InviteTo")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<Guid>("WorkspaceRoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UserId");

                    b.HasIndex("WorkspaceRoleId");

                    b.ToTable("InviteWorkspaceRoles");
                });

            modelBuilder.Entity("VADAR.Model.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Flag")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("VADAR.Model.Models.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("PermissionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("VADAR.Model.Models.Policy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Policies");
                });

            modelBuilder.Entity("VADAR.Model.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UpdateById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("VADAR.Model.Models.RolePermission", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions");
                });

            modelBuilder.Entity("VADAR.Model.Models.RoleUser", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("RoleUsers");
                });

            modelBuilder.Entity("VADAR.Model.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4")
                        .HasMaxLength(150);

                    b.Property<string>("ApprovedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("ApprovedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ApproverComment")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("FullName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsProfileUpdated")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("ApprovedById");

                    b.HasIndex("CountryId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VADAR.Model.Models.UserClaim", b =>
                {
                    b.Property<int>("ClaimId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("ClaimId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("VADAR.Model.Models.UserLanguage", b =>
                {
                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.HasKey("LanguageId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLanguages");
                });

            modelBuilder.Entity("VADAR.Model.Models.WhiteList", b =>
                {
                    b.Property<string>("Ip")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4")
                        .HasMaxLength(150);

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.HasKey("Ip");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("WhiteLists");
                });

            modelBuilder.Entity("VADAR.Model.Models.Workspace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("GrafanaDashboardId")
                        .HasColumnType("bigint");

                    b.Property<string>("GrafanaDashboardUrl")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GrafanaFolderUID")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("TokenWorkspace")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UpdateById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("WazuhRef")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ZabbixRef")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.ToTable("Workspaces");
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceHost", b =>
                {
                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.Property<Guid>("HostId")
                        .HasColumnType("char(36)");

                    b.HasKey("WorkspaceId", "HostId");

                    b.HasIndex("HostId");

                    b.ToTable("WorkspaceHosts");
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspacePolicy", b =>
                {
                    b.Property<int>("PolicyId")
                        .HasColumnType("int");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.HasKey("PolicyId", "WorkspaceId");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("WorkspacePolicies");
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UpdateById")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdateById");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("WorkspaceRoles");
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceRolePermission", b =>
                {
                    b.Property<Guid>("WorkspaceRoleId")
                        .HasColumnType("char(36)");

                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint");

                    b.HasKey("WorkspaceRoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("WorkspaceRolePermissions");
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceRoleUser", b =>
                {
                    b.Property<Guid>("WorkspaceRoleId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(150) CHARACTER SET utf8mb4");

                    b.HasKey("WorkspaceRoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("WorkspaceRoleUsers");
                });

            modelBuilder.Entity("VADAR.Model.Models.Claim", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");
                });

            modelBuilder.Entity("VADAR.Model.Models.Group", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");

                    b.HasOne("VADAR.Model.Models.Workspace", "Workspace")
                        .WithMany()
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.GroupHost", b =>
                {
                    b.HasOne("VADAR.Model.Models.Group", "Group")
                        .WithMany("GroupHosts")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.Host", "Host")
                        .WithMany("GroupHosts")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.Host", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");
                });

            modelBuilder.Entity("VADAR.Model.Models.InviteWorkspaceRole", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "InvitedUser")
                        .WithMany("InviteWorkspaceRoles")
                        .HasForeignKey("UserId");

                    b.HasOne("VADAR.Model.Models.WorkspaceRole", "WorkspaceRole")
                        .WithMany("InviteWorkspaceRoles")
                        .HasForeignKey("WorkspaceRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.Role", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");
                });

            modelBuilder.Entity("VADAR.Model.Models.RolePermission", b =>
                {
                    b.HasOne("VADAR.Model.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.RoleUser", b =>
                {
                    b.HasOne("VADAR.Model.Models.Role", "Role")
                        .WithMany("RoleUsers")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.User", "User")
                        .WithMany("RoleUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.User", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "ApprovedBy")
                        .WithMany()
                        .HasForeignKey("ApprovedById");

                    b.HasOne("VADAR.Model.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.UserClaim", b =>
                {
                    b.HasOne("VADAR.Model.Models.Claim", "Claim")
                        .WithMany()
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.User", "User")
                        .WithMany("UserClaims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.UserLanguage", b =>
                {
                    b.HasOne("VADAR.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.User", "User")
                        .WithMany("UserLanguages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.WhiteList", b =>
                {
                    b.HasOne("VADAR.Model.Models.Workspace", "Workspace")
                        .WithMany("WhiteLists")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.Workspace", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceHost", b =>
                {
                    b.HasOne("VADAR.Model.Models.Host", "Host")
                        .WithMany("WorkspaceHosts")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.Workspace", "Workspace")
                        .WithMany()
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspacePolicy", b =>
                {
                    b.HasOne("VADAR.Model.Models.Policy", "Policy")
                        .WithMany("WorkspacePolicies")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.Workspace", "Workspace")
                        .WithMany("WorkspacePolicies")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceRole", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("VADAR.Model.Models.User", "UpdateBy")
                        .WithMany()
                        .HasForeignKey("UpdateById");

                    b.HasOne("VADAR.Model.Models.Workspace", "Workspace")
                        .WithMany("WorkspaceRoles")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceRolePermission", b =>
                {
                    b.HasOne("VADAR.Model.Models.Permission", "Permission")
                        .WithMany("WorkspaceRolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.WorkspaceRole", "WorkspaceRole")
                        .WithMany("WorkspaceRolePermissions")
                        .HasForeignKey("WorkspaceRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VADAR.Model.Models.WorkspaceRoleUser", b =>
                {
                    b.HasOne("VADAR.Model.Models.User", "User")
                        .WithMany("WorkspaceRoleUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VADAR.Model.Models.WorkspaceRole", "WorkspaceRole")
                        .WithMany("WorkspaceRoleUsers")
                        .HasForeignKey("WorkspaceRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
