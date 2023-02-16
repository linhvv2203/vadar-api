// <copyright file="IInviteWorkspaceRoleRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// InviteWorkspaceRole Repository Interface.
    /// </summary>
    public interface IInviteWorkspaceRoleRepository : IGenericRepository<InviteWorkspaceRole>
    {
        /// <summary>
        /// Get InviteWorkspaceRole By Id.
        /// </summary>
        /// <param name="invitationId">invitationId.</param>
        /// <returns>InviteWorkspaceRole.</returns>
        Task<InviteWorkspaceRole> GetInviteWorkspaceRoleById(Guid invitationId);
    }
}
