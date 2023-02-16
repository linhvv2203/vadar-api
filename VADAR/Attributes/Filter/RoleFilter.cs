// <copyright file="RoleFilter.cs" company="VSEC">
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
    /// Role Filter Attribute.
    /// </summary>
    public class RoleFilter : Attribute, IAsyncActionFilter
    {
        private readonly Guid[] roleIds;

        /// <summary>
        /// Initialises a new instance of the <see cref="RoleFilter"/> class.
        /// Permission Filter.
        /// </summary>
        /// <param name="roleIds">Permission Id.</param>
        public RoleFilter(params Guid[] roleIds)
        {
            this.roleIds = roleIds;
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

            var existingRoleids = await roleService.GetRoleIdsByUserId(user["sub"].Value<string>());

            if (existingRoleids.Intersect(this.roleIds).Count() <= 0)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await next();
        }
    }
}
