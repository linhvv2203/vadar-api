// <copyright file="UserDataFilter.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Service.Interfaces;
using static VADAR.Helpers.Const.Constants;

namespace VADAR.WebAPI.Attributes.Filter
{
    /// <summary>
    /// User data filter.
    /// </summary>
    public sealed class UserDataFilter : Attribute, IAsyncActionFilter
    {
        private IMapper mapper;
        private IConfiguration configuration;
        private IUserService userService;
        private IRedisCachingHelper redisCachingHelper;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserDataFilter"/> class.
        /// Permission Filter.
        /// </summary>
        public UserDataFilter()
        {
        }

        /// <summary>
        /// OnActionExecutionAsync.
        /// </summary>
        /// <param name="context">context.</param>
        /// <param name="next">next.</param>
        /// <returns>Task Completed.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.FindFirst("sub") != null)
            {
                this.redisCachingHelper = (IRedisCachingHelper)context.HttpContext.RequestServices.GetService(typeof(IRedisCachingHelper));
                this.userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService));
                this.configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
                this.mapper = (IMapper)context.HttpContext.RequestServices.GetService(typeof(IMapper));
                var userId = context.HttpContext.User.FindFirst("sub");
                UserDto currentUser = null;
                var redisCacheData = await this.redisCachingHelper.GetDataByKey(PrefixCache.PrefixUser + userId.Value);

                if (!string.IsNullOrEmpty(redisCacheData))
                {
                    currentUser = JsonConvert.DeserializeObject<UserDto>(redisCacheData);
                    if (currentUser == null)
                    {
                        await this.redisCachingHelper.RemoveByKey(PrefixCache.PrefixUser + userId.Value);
                        redisCacheData = string.Empty;
                    }
                }

                if (string.IsNullOrWhiteSpace(redisCacheData))
                {
                    try
                    {
                        // authorize request source here.
                        if (context.HttpContext.Request.Headers["Authorization"].Count > 0 && context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationToken))
                        {
                            if (authorizationToken.Count <= 0 || authorizationToken.ToString().Length <= 25)
                            {
                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return;
                            }

                            using var client = new HttpClient
                            {
                                BaseAddress = new Uri(this.configuration["IdentityServerSetting:IdentityServerUrl"]),
                            };
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            string token = authorizationToken;
                            if (token.Length < 9)
                            {
                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return;
                            }

                            token = token.Remove(0, 7).Trim();
                            client.Timeout = TimeSpan.FromMinutes(30);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            var response = client.GetAsync("/connect/userinfo").Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var userString = response.Content.ReadAsStringAsync().Result;
                                context.HttpContext.Request.Headers.Remove("UserInfo");
                                context.HttpContext.Request.Headers.Add("UserInfo", userString);

                                var user = JObject.Parse(userString);
                                if (string.IsNullOrEmpty(user.Value<string>("sub")))
                                {
                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    return;
                                }

                                if (string.IsNullOrEmpty(user.Value<string>("email")))
                                {
                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation;
                                    return;
                                }

                                var userDto = this.UserSynchronization(user, (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService)));

                                if (userDto == null)
                                {
                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    return;
                                }

                                if (this.ForbiddenFilterConditions(userDto, context.HttpContext))
                                {
                                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                    return;
                                }
                            }
                            else
                            {
                                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return;
                            }
                        }
                        else
                        {
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // SentrySdk.CaptureException(ex);
                        if (ex.InnerException != null && ex.InnerException.HResult == (int)HttpStatusCode.Forbidden)
                        {
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        }
                        else
                        {
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }

                        var data = Encoding.UTF8.GetBytes(ex.Message);
                        context.HttpContext.Response.ContentType = "application/json";
                        await context.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                        return;
                    }
                }
                else
                {
                    if (this.ForbiddenFilterConditions(currentUser, context.HttpContext))
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }

                    context.HttpContext.Request.Headers.Remove("UserInfo");
                    context.HttpContext.Request.Headers.Add("UserInfo", JsonConvert.SerializeObject(new { email = currentUser?.Email, sub = currentUser?.Id }));
                }
            }

            await next();
        }

        /// <summary>
        /// User Synchronization Method.
        /// </summary>
        /// <param name="user">User Information.</param>
        /// <param name="userService">User Service.</param>
        /// <returns>User Dto.</returns>
        private UserDto UserSynchronization(JObject user, IUserService userService)
        {
            UserDto currentUser = null;
            var redisCacheData = this.redisCachingHelper.GetDataByKey(PrefixCache.PrefixUser + user.Value<string>("sub")).Result;
            if (string.IsNullOrWhiteSpace(redisCacheData)
                && userService != null)
            {
                // Synchronize user data
                currentUser = new UserDto
                {
                    Id = user.Value<string>(JwtClaimTypes.Subject),
                    UserName = user.Value<string>(JwtClaimTypes.PreferredUserName),
                    IsProfileUpdated = false,
                    CountryId = 237,
                    Email = user.Value<string>(JwtClaimTypes.Email),
                    FullName = user.Value<string>(JwtClaimTypes.Name),
                };

                currentUser = userService.AddUserIfNotExist(currentUser).Result;

                // Add the user into cache storage
                this.redisCachingHelper.SetObjectData(PrefixCache.PrefixUser + user.Value<string>("sub"), (UserDto)currentUser, 900);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(redisCacheData))
                {
                    currentUser = JsonConvert.DeserializeObject<UserDto>(redisCacheData);
                }
            }

            return currentUser;
        }

        private bool ForbiddenFilterConditions(UserDto currentUser, HttpContext context)
        {
            return currentUser?.Status == (int)EnUserStatus.Cancel
               && !(context.Request.Path.HasValue && (context.Request.Path.Value.ToLower().Contains("getprofile")
                   || context.Request.Path.Value.ToLower().Contains("getnotificationsbyuserid")));
        }
    }
}
