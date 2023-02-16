// <copyright file="IUserRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// User Repository Interface.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <returns>User.</returns>
        Task<User> GetUserById(string userId);
    }
}
