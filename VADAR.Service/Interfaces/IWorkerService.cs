// <copyright file="IWorkerService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Worker service Interface.
    /// </summary>
    public interface IWorkerService
    {
        /// <summary>
        /// ReceiveMessages.
        /// </summary>
        /// <returns>Task.</returns>
        Task ReceiveMessages();

        /// <summary>
        /// WorkerChangeStatusLicense.
        /// </summary>
        /// <returns>T.</returns>
        Task WorkerChangeStatusLicense();

        /// <summary>
        /// CheckEmailReport.
        /// </summary>
        /// <returns>T.</returns>
        Task CheckEmailReport();
    }
}
