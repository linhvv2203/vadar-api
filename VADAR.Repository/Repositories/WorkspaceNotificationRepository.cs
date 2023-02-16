// <copyright file="WorkspaceNotificationRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Workspace Notification Repository.
    /// </summary>
    public class WorkspaceNotificationRepository : GenericRepository<WorkspaceNotification>, IWorkspaceNotificationRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WorkspaceNotificationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public WorkspaceNotificationRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
