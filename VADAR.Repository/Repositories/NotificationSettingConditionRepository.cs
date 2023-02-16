// <copyright file="NotificationSettingConditionRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Notification Setting Repository.
    /// </summary>
    public class NotificationSettingConditionRepository : GenericRepository<NotificationSettingCondition>, INotificationSettingConditionRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="NotificationSettingConditionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public NotificationSettingConditionRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
