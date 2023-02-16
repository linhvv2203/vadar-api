// <copyright file="NotificationSettingRepository.cs" company="VSEC">
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
    public class NotificationSettingRepository : GenericRepository<NotificationSetting>, INotificationSettingRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="NotificationSettingRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public NotificationSettingRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
