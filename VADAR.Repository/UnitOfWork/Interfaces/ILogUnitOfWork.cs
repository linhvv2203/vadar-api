// <copyright file="ILogUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Log Unit Of Work Interface.
    /// </summary>
    public interface ILogUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets role Role Permission Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role Permission Repository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }

        /// <summary>
        /// Gets Group Repository.
        /// </summary>
        IGroupRepository GroupRepository { get; }

        /// <summary>
        /// Gets Role Repository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets WorkspaceRoleUserRepository.
        /// </summary>
        IWorkspaceRoleUserRepository WorkspaceRoleUserRepository { get; }

        /// <summary>
        /// Gets WorkspaceRoleRepository.
        /// </summary>
        IWorkspaceRoleRepository WorkspaceRoleRepository { get; }

        /// <summary>
        /// Gets hostRepository.
        /// </summary>
        IHostRepository HostRepository { get; }

        /// <summary>
        /// Gets workspaceHostRepository.
        /// </summary>
        IWorkspaceHostRepository WorkspaceHostRepository { get; }

        /// <summary>
        /// Gets GroupHostRepository.
        /// </summary>
        IGroupHostRepository GroupHostRepository { get; }
    }
}
