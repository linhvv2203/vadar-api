// <copyright file="LoggerHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Logging;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// LoggerHelper.
    /// </summary>
    /// <typeparam name="T">Class.</typeparam>
    public class LoggerHelper<T> : ILoggerHelper<T>
        where T : class
    {
        /// <summary>
        /// ILogger.
        /// </summary>
        private readonly ILogger<T> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="LoggerHelper{T}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public LoggerHelper(ILogger<T> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Log Error.
        /// </summary>
        /// <param name="eventId">The eventId.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void LogError(EventId eventId, Exception exception, string message)
        {
            try
            {
                // SentrySdk.CaptureException(exception);
                this.logger.LogError(eventId, exception, message);
            }
            catch
            {
                // ignored
            }
        }

        /// <inheritdoc/>
        public void LogInfo(string message)
        {
            try
            {
                this.logger.LogInformation(message);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
