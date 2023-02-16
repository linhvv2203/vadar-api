// <copyright file="IGroupRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// Group Repository Interface.
    /// </summary>
    public interface IGroupRepository : IGenericRepository<Group>
    {
        /// <summary>
        /// Get group by id.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Group> GetGroupById(Guid id);

        /// <summary>
        /// Get Group by Name.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>group.</returns>
        Task<Group> GetGroupByName(string name);
    }
}
