// <copyright file="IWorkspaceRoleRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// Workspace Role Repository Interface.
    /// </summary>
    public interface IWorkspaceRoleRepository : IGenericRepository<WorkspaceRole>
    {
        /// <summary>
        /// Get Workspace Role by Workspace id.
        /// </summary>
        /// <param name="workspaceId">Workspace id.</param>
        /// <returns>IQueryable of Workspace Role.</returns>
        Task<IQueryable<WorkspaceRole>> GetWorkspaceRoleByWorkspaceId(int workspaceId);

        /// <summary>
        /// Get Workspace Role By Id.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Workspace Role.</returns>
        Task<WorkspaceRole> GetWorkspaceRoleById(Guid id);
    }
}
