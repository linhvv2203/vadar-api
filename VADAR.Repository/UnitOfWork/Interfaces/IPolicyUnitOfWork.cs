// <copyright file="IPolicyUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Policy Unit Of Work Interface.
    /// </summary>
    public interface IPolicyUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Policy Repository.
        /// </summary>
        IPolicyRepository PolicyRepository { get; }

        /// <summary>
        /// Gets Workspace Policy Repository.
        /// </summary>
        IWorkspacePolicyRepository WorkspacePolicyRepository { get; }

        /// <summary>
        /// Gets WhiteList Repository.
        /// </summary>
        IWhiteListRepository WhiteListRepository { get; }

        /// <summary>
        /// Gets Role Repository.
        /// </summary>
        IHostRepository HostRepository { get; }

        /// <summary>
        /// Gets Workspace Host Repository.
        /// </summary>
        IWorkspaceHostRepository WorkspaceHostRepository { get; }

        /// <summary>
        /// Gets Workspace Repository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets RolePermission Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets WorkspaceRolePermission Repository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }
    }
}
