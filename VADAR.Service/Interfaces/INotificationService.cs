// <copyright file="INotificationService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Notification Service Interface.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send Notification.
        /// </summary>
        /// <param name="sendNotificationRequest">sendNotificationRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> SendNotification(SendNotificationRequest sendNotificationRequest);
    }
}
