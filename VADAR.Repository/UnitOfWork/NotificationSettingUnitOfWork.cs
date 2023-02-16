// <copyright file="NotificationSettingUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;
using VADAR.Repository.Repositories;
using VADAR.Repository.UnitOfWork.Interfaces;

namespace VADAR.Repository.UnitOfWork
{
    /// <summary>
    /// Notification Setting UnitOfWork.
    /// </summary>
    public class NotificationSettingUnitOfWork : UnitOfWorkBase, INotificationSettingUnitOfWork
    {
        private INotificationSettingRepository notificationSettingRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="NotificationSettingUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public NotificationSettingUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public INotificationSettingRepository NotificationSettingRepository => this.notificationSettingRepository ?? (this.notificationSettingRepository = new NotificationSettingRepository(this.dbContext));
    }
}
