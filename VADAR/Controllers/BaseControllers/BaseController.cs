// <copyright file="BaseController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VADAR.WebAPI.Attributes.Filter;

namespace VADAR.WebAPI.Controllers.BaseControllers
{
    /// <summary>
    /// BaseController.
    /// </summary>
    // [ApiVersion("1.0")]
    [ServiceFilter(typeof(VADARExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [UserDataFilter]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets get Current User Id.
        /// </summary>
        /// <returns>user Id.</returns>
        protected string CurrentUserId => this.HttpContext.Request.Headers["UserInfo"].Count > 0 ? JObject.Parse(this.HttpContext.Request.Headers["UserInfo"]).Value<string>("sub").Trim() : string.Empty;

        /// <summary>
        /// Gets get Current User Email.
        /// </summary>
        /// <returns>user email.</returns>
        protected string CurrentUserEmail => this.HttpContext.Request.Headers["UserInfo"].Count > 0 ? JObject.Parse(this.HttpContext.Request.Headers["UserInfo"]).Value<string>("email").Trim() : string.Empty;
    }
}
