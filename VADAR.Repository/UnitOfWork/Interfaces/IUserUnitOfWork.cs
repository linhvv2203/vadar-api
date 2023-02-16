// <copyright file="IUserUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// User Unit Of Work Interface.
    /// </summary>
    public interface IUserUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets User Repository.
        /// </summary>
        IUserRepository UserRepository { get; }

        /// <summary>
        /// Gets RoleUser Repository.
        /// </summary>
        IRoleUserRepository RoleUserRepository { get; }

        /// <summary>
        /// Gets Role Permision Repository.
        /// </summary>
        IRolePermissionRepository RolePermissionRepository { get; }

        /// <summary>
        /// Gets WorkspaceRolePermissionRepository.
        /// </summary>
        IWorkspaceRolePermissionRepository WorkspaceRolePermissionRepository { get; }

        /// <summary>
        /// Gets UserClaims Repository.
        /// </summary>
        IUserClaimRepository UserClaimsRepository { get; }
    }
}
