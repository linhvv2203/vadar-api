// <copyright file="IWorkspaceRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// Workspace Repository Interface.
    /// </summary>
    public interface IWorkspaceRepository : IGenericRepository<Workspace>
    {
        /// <summary>
        /// Get workspace by id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>Workspace.</returns>
        Task<Workspace> GetWorkspaceById(int id);

        /// <summary>
        /// Get workspace by id.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <returns>Workspace.</returns>
        Task<Workspace> GetWorkspaceByToken(string token);
    }
}
