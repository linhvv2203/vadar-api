// <copyright file="NotificationSettingConditionUnitOfWork.cs" company="VSEC">
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
    /// Notification Setting Condition UnitOfWork.
    /// </summary>
    public class NotificationSettingConditionUnitOfWork : UnitOfWorkBase, INotificationSettingConditionUnitOfWork
    {
        private INotificationSettingConditionRepository notificationSettingConditionRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="NotificationSettingConditionUnitOfWork"/> class.
        /// </summary>
        /// <param name="dbcontext">dbcontext.</param>
        public NotificationSettingConditionUnitOfWork(IDbContext dbcontext)
            : base(dbcontext)
        {
        }

        /// <inheritdoc/>
        public INotificationSettingConditionRepository NotificationSettingConditionRepository => this.notificationSettingConditionRepository ?? (this.notificationSettingConditionRepository = new NotificationSettingConditionRepository(this.dbContext));
    }
}
