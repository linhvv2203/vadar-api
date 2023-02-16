// <copyright file="IHostRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// Host Repository Interface.
    /// </summary>
    public interface IHostRepository : IGenericRepository<Host>
    {
        /// <summary>
        /// Gets Host By Name.
        /// </summary>
        /// <param name="hostName">hostName.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<Host> GetHostByName(string hostName);

        /// <summary>
        /// Gets Get Host By Id.
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <returns>A.</returns>
        Task<Host> GetHostById(Guid hostId);
    }
}
