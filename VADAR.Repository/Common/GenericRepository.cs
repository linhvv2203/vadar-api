// <copyright file="GenericRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VADAR.Model.Models;

namespace VADAR.Repository.Common
{
    /// <summary>
    /// GenericRepository.
    /// </summary>
    /// <typeparam name="T">Class model.</typeparam>
    public abstract class GenericRepository<T> : IGenericRepository<T>
    where T : class
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// Dbset.
        /// </summary>
        protected readonly DbSet<T> dbset;
#pragma warning restore SA1401 // Fields should be private

#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// Entities.
        /// </summary>
        protected readonly IDbContext entities;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initialises a new instance of the <see cref="GenericRepository{T}"/> class.
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="context">IDbcontext.</param>
        protected GenericRepository(IDbContext context)
        {
            this.entities = context;
            this.dbset = context.Set<T>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="GenericRepository{T}"/> class.
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        protected GenericRepository()
        {
        }

        /// <inheritdoc />
        public virtual async Task<IQueryable<T>> GetAll()
        {
            return await Task.FromResult(this.dbset);
        }

        /// <inheritdoc />
        public virtual async Task<IQueryable<T>> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            var query = this.dbset.Where(predicate);
            return await Task.FromResult(query);
        }

        /// <inheritdoc />
        public virtual async Task<T> Add(T entity)
        {
            return (await this.dbset.AddAsync(entity)).Entity;
        }

        /// <inheritdoc />
        public virtual async Task<T> Delete(T entity)
        {
            this.dbset.Attach(entity);
            return await Task.FromResult(this.dbset.Remove(entity).Entity);
        }

        /// <inheritdoc />
        public virtual async Task Edit(T entity)
        {
            if (this.entities != null)
            {
                await Task.FromResult(this.entities.Entry(entity).State = EntityState.Modified);
            }
        }
    }
}
