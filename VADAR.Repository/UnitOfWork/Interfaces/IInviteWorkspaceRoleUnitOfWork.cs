// <copyright file="IInviteWorkspaceRoleUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// InviteWorkspaceRole UnitOfWork Interface.
    /// </summary>
    public interface IInviteWorkspaceRoleUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets InviteWorkspaceRole Repository.
        /// </summary>
        IInviteWorkspaceRoleRepository InviteWorkspaceRoleRepository { get; }

        /// <summary>
        /// Gets Workspace Repository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets Workspace Role Repository.
        /// </summary>
        IWorkspaceRoleRepository WorkspaceRoleRepository { get; }

        /// <summary>
        /// Gets role Role Permission Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role Permission Repository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role User Repository.
        /// </summary>
        IWorkspaceRoleUserRepository WorkspaceRoleUserRepository { get; }

        /// <summary>
        /// Gets User Repository.
        /// </summary>
        IUserRepository UserRepository { get; }
    }
}
