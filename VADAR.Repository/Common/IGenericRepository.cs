// <copyright file="IGenericRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VADAR.Repository.Common
{
    /// <summary>
    /// IGenericRepository.
    /// </summary>
    /// <typeparam name="T">Model.</typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>
        /// Get all models.
        /// </summary>
        /// <returns>IQueryable T.</returns>
        Task<IQueryable<T>> GetAll();

        /// <summary>
        /// IEnumerable.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>T.</returns>
        Task<IQueryable<T>> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add.
        /// </summary>
        /// <param name="entity">entity.</param>
        /// <returns>T.</returns>
        Task<T> Add(T entity);

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>T.</returns>
        Task<T> Delete(T entity);

        /// <summary>
        /// Edit.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Task.</returns>
        Task Edit(T entity);
    }
}
