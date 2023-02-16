// <copyright file="INotificationSettingUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Notification Setting UnitOfWork Interface.
    /// </summary>
    public interface INotificationSettingUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Notification Setting Repository Contructor.
        /// </summary>
        INotificationSettingRepository NotificationSettingRepository { get; }
    }
}
