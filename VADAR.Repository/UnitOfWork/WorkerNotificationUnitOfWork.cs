// <copyright file="WorkerNotificationUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;
using VADAR.Repository.Repositories;
using VADAR.Repository.UnitOfWork.Interfaces;

namespace VADAR.Repository.UnitOfWork
{
    /// <summary>
    /// Worker Notification UnitOfWork.
    /// </summary>
    public class WorkerNotificationUnitOfWork : UnitOfWorkBase, IWorkerNotificationUnitOfWork
    {
        private IWorkspaceClaimRepository workspaceClaimRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkerNotificationUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbContext">context.</param>
        public WorkerNotificationUnitOfWork(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <inheritdoc/>
        public IWorkspaceClaimRepository WorkspaceClaimRepository => this.workspaceClaimRepository ??= new WorkspaceClaimRepository(this.dbContext);
    }
}
