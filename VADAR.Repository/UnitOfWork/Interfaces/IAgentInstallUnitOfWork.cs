// <copyright file="IAgentInstallUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Work Space Role Unit Of Work Interface.
    /// </summary>
    public interface IAgentInstallUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Agent Install Repository.
        /// </summary>
        IAgentInstallRepository AgentInstallRepository { get; }

        /// <summary>
        /// Gets workspaceRepository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets AgentOsRepository.
        /// </summary>
        IAgentOsRepository AgentOsRepository { get; }

        /// <summary>
        /// Gets RolePermissionRepository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets WorkspaceRolePermissionRepository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }
    }
}
