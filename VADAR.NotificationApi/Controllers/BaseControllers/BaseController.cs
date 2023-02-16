// <copyright file="BaseController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VADAR.NotificationApi.Attributes.Filter;

namespace VADAR.NotificationApi.Controllers.BaseControllers
{
    /// <summary>
    /// BaseController.
    /// </summary>
    [ServiceFilter(typeof(VADARExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets get Current User Id.
        /// </summary>
        /// <returns>user Id.</returns>
        protected string CurrentUserId => HttpContext.Request.Headers["UserInfo"].Count > 0 ? JObject.Parse(HttpContext.Request.Headers["UserInfo"]).Value<string>("sub").Trim() : string.Empty;

        /// <summary>
        /// Gets get Current User Email.
        /// </summary>
        /// <returns>user email.</returns>
        protected string CurrentUserEmail => HttpContext.Request.Headers["UserInfo"].Count > 0 ? JObject.Parse(HttpContext.Request.Headers["UserInfo"]).Value<string>("email").Trim() : string.Empty;
    }
}
