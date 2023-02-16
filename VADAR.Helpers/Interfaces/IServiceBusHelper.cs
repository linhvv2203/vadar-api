// <copyright file="IServiceBusHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Service Bus Helper Interface.
    /// </summary>
    public interface IServiceBusHelper
    {
        /// <summary>
        /// RecieveMessages.
        /// </summary>
        /// <returns>Task.</returns>
        Task RecieveMessages();

        /// <summary>
        /// Send Message.
        /// </summary>
        /// <param name="message">message object.</param>
        /// <returns>Task done.</returns>
        Task SendMessage(dynamic message);
    }
}
