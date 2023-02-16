// <copyright file="IRoleUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Interface Role Unit Of Work.
    /// </summary>
    public interface IRoleUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Role Repository Contructor.
        /// </summary>
        IRoleRepository RoleRepository { get; }

        /// <summary>
        /// Gets Permission Repository Contructor.
        /// </summary>
        IPermissionRepository PermissionRepository { get; }

        /// <summary>
        /// Gets RolePermission Repository Contructor.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets RoleUser Repository Contructor.
        /// </summary>
        IRoleUserRepository RoleUserRepository { get; }
    }
}
