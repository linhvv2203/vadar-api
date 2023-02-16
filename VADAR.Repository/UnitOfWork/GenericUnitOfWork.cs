// <copyright file="GenericUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;

namespace VADAR.Repository.UnitOfWork
{
    /// <summary>
    /// GenericUnitOfWork.
    /// </summary>
    /// <typeparam name="TRepository">Repository.</typeparam>
    /// <typeparam name="TInterface">Interface of repository.</typeparam>
    public class GenericUnitOfWork<TRepository, TInterface> : IGenericUnitOfWork<TInterface>
    where TRepository : TInterface
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// true means dbContext was disposed.
        /// </summary>
        protected bool disposed;
#pragma warning restore SA1401 // Fields should be private

#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1214 // Readonly fields should appear before non-readonly fields
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IDbContext DbContext;
#pragma warning restore SA1214 // Readonly fields should appear before non-readonly fields
#pragma warning restore SA1401 // Fields should be private

        private TInterface repository;

        /// <summary>
        /// Initialises a new instance of the <see cref="GenericUnitOfWork{TRepository, TInterface}"/> class.
        /// </summary>
        /// <param name="context">context.</param>
        public GenericUnitOfWork(IDbContext context)
        {
            this.DbContext = context;
        }

        /// <summary>
        /// Finalises an instance of the <see cref="GenericUnitOfWork{TRepository, TInterface}"/> class.
        /// </summary>
        ~GenericUnitOfWork()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets get Repository.
        /// </summary>
        public TInterface Repository
        {
            get
            {
                if (this.repository == null)
                {
                    this.repository = (TRepository)Activator.CreateInstance(typeof(TRepository), this.DbContext);
                }

                return this.repository;
            }
        }

        /// <inheritdoc />
        public async Task<int> Commit()
        {
            // Save changes with the default options
            return await this.DbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(true);
        }

        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.DbContext.Dispose();
            this.disposed = true;

            if (!disposing)
            {
                return;
            }

            GC.SuppressFinalize(this);
        }
    }
}
