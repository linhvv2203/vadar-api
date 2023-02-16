// <copyright file="INotificationSettingRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// Notification Setting Repository Interface.
    /// </summary>
    public interface INotificationSettingRepository : IGenericRepository<NotificationSetting>
    {
    }
}
