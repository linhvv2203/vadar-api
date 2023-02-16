// <copyright file="IDashboardUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Dashboard Unit Of Work Interface.
    /// </summary>
    public interface IDashboardUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Role Repository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets Role Repository.
        /// </summary>
        IRoleRepository RoleRepository { get; }

        /// <summary>
        /// Gets Permission Repository.
        /// </summary>
        IPermissionRepository PermissionRepository { get; }

        /// <summary>
        /// Gets RolePermission Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets RoleUser Repository.
        /// </summary>
        IRoleUserRepository RoleUserRepository { get; }

        /// <summary>
        /// Gets GroupHost Repository.
        /// </summary>
        IGroupHostRepository GroupHostRepository { get; }

        /// <summary>
        /// Gets Group Repository.
        /// </summary>
        IGroupRepository GroupRepository { get; }

        /// <summary>
        /// Gets Host Repository.
        /// </summary>
        IHostRepository HostRepository { get; }

        /// <summary>
        /// Gets workspaceHostRepository.
        /// </summary>
        IWorkspaceHostRepository WorkspaceHostRepository { get; }

        /// <summary>
        /// Gets workspaceRolePermissionRepository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }

        /// <summary>
        /// Gets WorkspaceRoleUserRepository.
        /// </summary>
        IWorkspaceRoleUserRepository WorkspaceRoleUserRepository { get; }

        /// <summary>
        /// Gets WorkspaceRoleRepository.
        /// </summary>
        IWorkspaceRoleRepository WorkspaceRoleRepository { get; }
    }
}
