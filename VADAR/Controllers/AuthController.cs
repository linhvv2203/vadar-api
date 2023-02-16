// <copyright file="AuthController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    [Produces("application/json")]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly ILoggerHelper<HostController> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        public AuthController(ILoggerHelper<HostController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Dummy API to fit with dummy api calls.
        /// </summary>
        /// <param name="dummyParam">Dummy Params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public ApiResponse<dynamic> DummyAPI(dynamic dummyParam)
        {
            try
            {
                return new ApiResponse<dynamic>(EnApiStatusCode.Success);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<dynamic>(ex.HResult);
            }
        }
    }
}
