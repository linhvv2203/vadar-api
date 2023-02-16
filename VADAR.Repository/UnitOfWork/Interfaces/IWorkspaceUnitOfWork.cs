// <copyright file="IWorkspaceUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Interface Role Unit Of Work.
    /// </summary>
    public interface IWorkspaceUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Workspace Notification Repository.
        /// </summary>
        IWorkspaceNotificationRepository WorkspaceNotificationRepository { get; }

        /// <summary>
        /// Gets Role Repository.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }

        /// <summary>
        /// Gets workspace Role Permission Repository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository
        {
            get;
        }

        /// <summary>
        /// Gets workspace Role Repository.
        /// </summary>
        IWorkspaceRoleRepository WorkspaceRoleRepository { get; }

        /// <summary>
        /// Gets workspace Role User Repository.
        /// </summary>
        IWorkspaceRoleUserRepository WorkspaceRoleUserRepository { get; }

        /// <summary>
        /// Gets role PermissionRepository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets AgentInstallRepository.
        /// </summary>
        IAgentInstallRepository AgentInstallRepository { get; }

        /// <summary>
        /// Gets AgentInstallWorkspaceRepository.
        /// </summary>
        IAgentOsRepository AgentOsRepository { get; }

        /// <summary>
        /// Gets InviteWorkspaceRoleRepository.
        /// </summary>
        IInviteWorkspaceRoleRepository InviteWorkspaceRoleRepository { get; }

        /// <summary>
        /// Gets NotificationSettingRepository.
        /// </summary>
        INotificationSettingRepository NotificationSettingRepository { get; }
    }
}
