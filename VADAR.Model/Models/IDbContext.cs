// <copyright file="IDbContext.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VADAR.Model.Models
{
    /// <summary>
    /// Interface for Database Context.
    /// </summary>
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// Dataset interface.
        /// </summary>
        /// <typeparam name="T">Data Type.</typeparam>
        /// <returns>Data Set.</returns>
        DbSet<T> Set<T>()
            where T : class;

        /// <summary>
        /// Save changes.
        /// </summary>
        /// <returns>Number records is effected.</returns>
        int SaveChanges();

        /// <summary>
        /// Get entity entry.
        /// </summary>
        /// <param name="o">Object.</param>
        /// <returns>Entity Entry Instance. </returns>
        EntityEntry Entry(object o);

        /// <summary>
        /// Save change async.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">accept all changes on success.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns>task int.</returns>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default(CancellationToken));
    }
}
