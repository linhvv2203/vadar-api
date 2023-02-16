// <copyright file="IWorkspaceRoleUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Work Space Role Unit Of Work Interface.
    /// </summary>
    public interface IWorkspaceRoleUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Role Repository WorkspaceRoleRepository.
        /// </summary>
        IWorkspaceRoleRepository WorkspaceRoleRepository { get; }

        /// <summary>
        /// Gets workspaceRepository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets Workspace Role Permission Repository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role User Repository.
        /// </summary>
        IWorkspaceRoleUserRepository WorkspaceRoleUserRepository { get; }

        /// <summary>
        /// Gets role Role Permission Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets role Permission Repository.
        /// </summary>
        IPermissionRepository PermissionRepository { get; }
    }
}
