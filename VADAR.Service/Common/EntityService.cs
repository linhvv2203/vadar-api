// <copyright file="EntityService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VADAR.Helpers.Enums;
using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common.Interfaces;
using static VADAR.Helpers.Const.Constants;

namespace VADAR.Service.Common
{
    /// <summary>
    /// EntityService.
    /// </summary>
    /// <typeparam name="T">Entity model.</typeparam>
    public abstract class EntityService<T> : IEntityService<T>
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IGenericRepository<T> repository;

        /// <summary>
        /// Initialises a new instance of the <see cref="EntityService{T}"/> class.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork.</param>
        /// <param name="repository">IGenericRepository.</param>
        protected EntityService(IUnitOfWork unitOfWork, IGenericRepository<T> repository)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EntityService{T}"/> class.
        /// </summary>
        /// <param name="unitOfWork">unitOfWork.</param>
        protected EntityService(IGroupUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public virtual async Task<T> Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await this.repository.Add(entity);
            return (await this.unitOfWork.Commit()) > 0 ? result : default(T);
        }

        /// <inheritdoc />
        public virtual async Task<bool> Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await this.repository.Edit(entity);
            return (await this.unitOfWork.Commit()) > 0;
        }

        /// <inheritdoc />
        public virtual async Task<bool> Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (await this.repository.Delete(entity) != null)
            {
                return await this.unitOfWork.Commit() > 0;
            }

            return true;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await this.repository.GetAll();
        }

        /// <inheritdoc />
        public virtual async Task<bool> ValidatePermission(string userId, int? workspaceId, long[] permissionIds, IRolePermissionRepository rolePermissionRepo, IWorkspaceRolePermissionRepository workspaceRolePermissionRepository)
        {
            var totalPermissions = await this.GetUserPermissions(
                userId,
                workspaceId,
                rolePermissionRepo,
                workspaceRolePermissionRepository);
            return totalPermissions.Any(p => permissionIds.Contains(p));
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<long>> GetPermissions(string userId, int? workspaceId, IRolePermissionRepository rolePermissionRepo, IWorkspaceRolePermissionRepository workspaceRolePermissionRepository)
        {
            return await this.GetUserPermissions(
                userId,
                workspaceId,
                rolePermissionRepo,
                workspaceRolePermissionRepository);
        }

        /// <inheritdoc />
        public List<AgentOs> GetDefaultAgentOs()
        {
            var agentOs = new List<AgentOs>
            {
                new AgentOs
                {
                    Name = Os.Ubuntu,
                    Icon = "https://dev.admin.vadar.vn/assets/images/host/ubuntu.svg",
                },
                new AgentOs
                {
                    Name = Os.Centos,
                    Icon = "https://dev.admin.vadar.vn/assets/images/host/centos.svg",
                },
                new AgentOs
                {
                    Name = Os.Window,
                    Icon = "https://dev.admin.vadar.vn/assets/images/host/windows.svg",
                },
                new AgentOs
                {
                    Name = Os.Macos,
                    Icon = "https://dev.admin.vadar.vn/assets/images/host/macos.svg",
                },
            };

            return agentOs;
        }

        private async Task<IEnumerable<long>> GetUserPermissions(string userId, int? workspaceId, IRolePermissionRepository rolePermissionRepo, IWorkspaceRolePermissionRepository workspaceRolePermissionRepository)
        {
            var permissions = await rolePermissionRepo.GetPermissionIdsByUserId(userId);

            if (workspaceId == 0 && !permissions.Any(p => p == (long)EnPermissions.FullPermission))
            {
                return new List<long>();
            }

            if (workspaceId != null && workspaceId > 0)
            {
                var wpPermission = (await workspaceRolePermissionRepository.GetAll())
                    .Where(wpr => wpr.WorkspaceRole.WorkspaceRoleUsers.Any(ru => ru.UserId == userId))
                    .Where(wpr => wpr.WorkspaceRole.WorkspaceId == workspaceId)
                    .Select(wpr => wpr.PermissionId);

                if (permissions.Any(p => p == (long)EnPermissions.FullPermission))
                {
                    return permissions.Union(wpPermission).ToList();
                }

                return wpPermission.ToList();
            }

            return permissions.ToList();
        }
    }
}
