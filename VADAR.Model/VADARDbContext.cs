// <copyright file="VADARDbContext.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VADAR.Model.Models;

namespace VADAR.Model
{
    /// <summary>
    /// DB Context.
    /// </summary>
    public class VADARDbContext : DbContext, IDbContext
    {
        private readonly bool isMemoryDatabase;

        /// <summary>
        /// Initialises a new instance of the <see cref="VADARDbContext"/> class.
        /// </summary>
        /// <param name="configurations">configurations.</param>
        /// <param name="isMemoryDatabase">isMemoryDatabase.</param>
        public VADARDbContext(IConfiguration configurations, bool isMemoryDatabase = false)
        {
            this.Configs = configurations;
            this.isMemoryDatabase = isMemoryDatabase;
        }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Country> Countries { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Workspace> Workspaces { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Language> Languages { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<UserLanguage> UserLanguages { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Claim> Claims { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<UserClaim> UserClaims { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<RoleUser> RoleUsers { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspaceRolePermission> WorkspaceRolePermissions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspaceRole> WorkspaceRoles { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspaceRoleUser> WorkspaceRoleUsers { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Host> Hosts { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<GroupHost> GroupHosts { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<InviteWorkspaceRole> InviteWorkspaceRoles { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspaceHost> WorkspaceHosts { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<Policy> Policies { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WhiteIp> WhiteIps { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspacePolicy> WorkspacePolicies { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<AgentOs> AgentOs { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<AgentInstall> AgentInstalls { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<License> Licenses { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspaceNotification> WorkspaceNotifications { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<NotificationSettingCondition> NotificationSettingConditions { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        public virtual DbSet<WorkspaceClaim> WorkspaceClaims { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        protected IConfiguration Configs { get; set; }

        /// <summary>
        /// On Configuration event.
        /// </summary>
        /// <param name="optionsBuilder">Builder options.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This process for mockup in test case
            if (this.isMemoryDatabase)
            {
                optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                return;
            }

            if (!optionsBuilder.IsConfigured && this.Configs != null)
            {
                optionsBuilder.UseMySql(string.IsNullOrEmpty(this.Configs["ConnectionStrings:VADARConnection"]) ? this.Configs.GetConnectionString("VSECConnection") : this.Configs["ConnectionStrings:VADARConnection"]);

                // optionsBuilder.UseSecondLevelCache();
            }
        }

        /// <summary>
        /// On Model Creating event.
        /// </summary>
        /// <param name="modelBuilder">Builder model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserClaim>()
                .HasKey(c => new { c.ClaimId, c.UserId });

            modelBuilder.Entity<RoleUser>()
                .HasKey(c => new { c.RoleId, c.UserId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(c => new { c.RoleId, c.PermissionId });

            modelBuilder.Entity<UserLanguage>()
                .HasKey(c => new { c.LanguageId, c.UserId });

            modelBuilder.Entity<WorkspaceRolePermission>()
                .HasKey(c => new { c.WorkspaceRoleId, c.PermissionId });

            modelBuilder.Entity<GroupHost>()
                .HasKey(c => new { c.HostId, c.GroupId });

            modelBuilder.Entity<WorkspaceRoleUser>()
                .HasKey(c => new { c.WorkspaceRoleId, c.UserId });

            modelBuilder.Entity<WorkspacePolicy>()
                .HasKey(c => new { c.PolicyId, c.WorkspaceId });

            modelBuilder.Entity<WhiteIp>()
                .HasKey(c => new { c.Ip, c.WorkspaceId });

            modelBuilder.Entity<WorkspaceHost>()
               .HasKey(c => new { c.WorkspaceId, c.HostId });
            modelBuilder.Entity<InviteWorkspaceRole>().HasOne(i => i.InvitedUser)
                .WithMany(u => u.InviteWorkspaceRoles);

            modelBuilder.Entity<User>().HasIndex(c => c.Email).IsUnique();

            modelBuilder.Entity<User>().HasIndex(c => c.UserName).IsUnique();
        }
    }
}
