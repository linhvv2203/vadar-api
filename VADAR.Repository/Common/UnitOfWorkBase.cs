// <copyright file="UnitOfWorkBase.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using VADAR.Model.Models;

namespace VADAR.Repository.Common
{
    /// <summary>
    /// The Entity Framework implementation of IUnitOfWork.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// true means dbContext was disposed.
        /// </summary>
        protected bool disposed;
#pragma warning restore SA1401 // Fields should be private

#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected IDbContext dbContext;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initialises a new instance of the <see cref="UnitOfWorkBase"/> class.
        /// </summary>
        /// <param name="context">object context.</param>
        public UnitOfWorkBase(IDbContext context)
        {
            this.dbContext = context;
        }

        /// <summary>
        /// Finalises an instance of the <see cref="UnitOfWorkBase"/> class.
        /// </summary>
        ~UnitOfWorkBase()
        {
            this.Dispose(false);
        }

        /// <inheritdoc />
        public async Task<int> Commit()
        {
            return await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.dbContext.Dispose();
            this.disposed = true;

            if (!disposing)
            {
                return;
            }
        }
    }
}
