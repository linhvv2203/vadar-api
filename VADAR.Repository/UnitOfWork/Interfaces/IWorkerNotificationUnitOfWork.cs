// <copyright file="IWorkerNotificationUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// WorkerNotification UnitOfWork Interface.
    /// </summary>
    public interface IWorkerNotificationUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets WorkspaceClaim Repository.
        /// </summary>
        IWorkspaceClaimRepository WorkspaceClaimRepository { get; }
    }
}
