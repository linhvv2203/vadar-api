// <copyright file="IWorkerNotificationService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// WorkerNotification Service Interface.
    /// </summary>
    public interface IWorkerNotificationService
    {
        /// <summary>
        /// ReceiveMessages.
        /// </summary>
        void ReceiveMessages();

        /// <summary>
        /// Get ChatIds From Database.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<long>> GetChatIdsFromDatabase(int workspaceId);

        /// <summary>
        /// AddChatIdsToDatabase.
        /// </summary>
        /// <param name="chatIds">chatIds.</param>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> AddChatIdsToDatabase(IEnumerable<long> chatIds, int workspaceId);
    }
}
