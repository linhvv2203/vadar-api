// <copyright file="IGroupUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Interface Role Unit Of Work.
    /// </summary>
    public interface IGroupUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Role Repostory Contructor.
        /// </summary>
        IGroupRepository GroupRepository { get; }

        /// <summary>
        /// Gets Role Permission Repository Contructor.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role Permission Repository Contructor.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }
    }
}
