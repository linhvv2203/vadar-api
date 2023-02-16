// <copyright file="IHostUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Interface Role Unit of Work.
    /// </summary>
    public interface IHostUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Role Repositoty Contructor.
        /// </summary>
        IHostRepository HostRepository { get; }

        /// <summary>
        /// Gets Workspace Host Repositoty Contructor.
        /// </summary>
        IWorkspaceHostRepository WorkspaceHostRepository { get; }

        /// <summary>
        /// Gets Group Host Repositoty Contructor.
        /// </summary>
        IGroupHostRepository GroupHostRepository { get; }

        /// <summary>
        /// Gets Role Permission Repository Contructor.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role Permission Repository Contructor.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }

        /// <summary>
        /// Gets Group Repositoty Contructor.
        /// </summary>
        IGroupRepository GroupRepository { get; }

        /// <summary>
        /// Gets Workspace Repository Contructor.
        /// </summary>
        IWorkspaceRepository WorkspaceRepository { get; }
    }
}
