// <copyright file="VADARExceptionFilter.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

// using Sentry;
namespace VADAR.WebAPI.Attributes.Filter
{
    /// <summary>
    /// VADAR Exception Filter.
    /// </summary>
    public sealed class VADARExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<VADARExceptionFilter> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="VADARExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public VADARExceptionFilter(ILogger<VADARExceptionFilter> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// OnException.
        /// </summary>
        /// <param name="context">Objet Context.</param>
        public override void OnException(ExceptionContext context)
        {
            // SentrySdk.CaptureException(context.Exception);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            this.logger.LogError(new EventId(0), context.Exception, context.Exception.GetBaseException().Message);

            base.OnException(context);
        }
    }
}
