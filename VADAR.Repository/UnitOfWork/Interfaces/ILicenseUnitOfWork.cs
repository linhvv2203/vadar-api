// <copyright file="ILicenseUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// License UnitOfWork Interface.
    /// </summary>
    public interface ILicenseUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets License Repository.
        /// </summary>
        ILicenseRepository LicenseRepository { get; }

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
