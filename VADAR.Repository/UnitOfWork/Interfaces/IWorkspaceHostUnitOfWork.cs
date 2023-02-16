// <copyright file="IWorkspaceHostUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Work Space Host Unit Of Work Interface.
    /// </summary>
    public interface IWorkspaceHostUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets WorkspaceHostRepository Contructor.
        /// </summary>
        IWorkspaceHostRepository WorkspaceHostRepository { get; }

        /// <summary>
        /// Gets role Role Permission Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets Workspace Role Permission Repository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }
    }
}
