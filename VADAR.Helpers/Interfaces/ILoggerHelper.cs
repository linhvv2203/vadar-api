// <copyright file="ILoggerHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Logging;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Interface Of LoggerHelper.
    /// </summary>
    /// <typeparam name="T">T.</typeparam>
    public interface ILoggerHelper<T>
    {
        /// <summary>
        /// Log Error.
        /// </summary>
        /// <param name="eventId">The EventId.</param>
        /// <param name="exception">The Exception.</param>
        /// <param name="message">The Message.</param>
        void LogError(EventId eventId, Exception exception, string message);

        /// <summary>
        /// Log Infomation.
        /// </summary>
        /// <param name="message">message.</param>
        void LogInfo(string message);
    }
}
