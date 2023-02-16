// <copyright file="INotificationSettingConditionUnitOfWork.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.UnitOfWork.Interfaces
{
    /// <summary>
    /// Notification Setting Condition UnitOfWork Interface.
    /// </summary>
    public interface INotificationSettingConditionUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets Notification Setting Repository Contructor.
        /// </summary>
        INotificationSettingConditionRepository NotificationSettingConditionRepository { get; }
    }
}
