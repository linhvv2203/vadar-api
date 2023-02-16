// <copyright file="PermissionFilter.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using VADAR.Service.Interfaces;

namespace VADAR.WebAPI.Attributes.Filter
{
    /// <summary>
    /// Permission Filter Class.
    /// </summary>
    public class PermissionFilter : Attribute, IAsyncActionFilter
    {
        private readonly long[] permissions;

        /// <summary>
        /// Initialises a new instance of the <see cref="PermissionFilter"/> class.
        /// Permission Filter.
        /// </summary>
        /// <param name="permissionIds">Permission Id.</param>
        public PermissionFilter(params long[] permissionIds)
        {
            this.permissions = permissionIds;
        }

        /// <summary>
        /// Permission Filteration.
        /// </summary>
        /// <param name="context">action context.</param>
        /// <param name="next">next execution delegate.</param>
        /// <returns>Task Completed.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Before action execution.
            var roleService = (IRoleService)context.HttpContext.RequestServices.GetService(typeof(IRoleService));
            var user = JObject.Parse(context.HttpContext.Request.Headers["UserInfo"]);

            if (user == null || user["sub"] == null || string.IsNullOrEmpty(user["sub"].Value<string>()))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            var existingPermissions = await roleService.GetPermissionIdsByUserId(user["sub"].Value<string>());

            if (!existingPermissions.Intersect(this.permissions).Any())
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await next();
        }
    }
}
