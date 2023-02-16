// <copyright file="UserController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ILoggerHelper<UserController> logger;
        private readonly IUserService userService;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">The _logger.</param>
        /// <param name="userService">User service.</param>
        public UserController(
            ILoggerHelper<UserController> logger,
            IUserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        /// <summary>
        /// Get User base information APi.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>User Base Information.</returns>
        [HttpGet]
        [Route("GetUserBaseInfo")]
        public async Task<ApiResponse<UserBaseInfoDto>> GetUserBaseInfo(int? workspaceId)
        {
            try
            {
                return new ApiResponse<UserBaseInfoDto>(EnApiStatusCode.Success, await this.userService.GetUserBaseInformation(workspaceId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<UserBaseInfoDto>(ex.HResult);
            }
        }

        /// <summary>
        /// Logout All Services.
        /// </summary>
        /// <returns>True: Success; False: Failed.</returns>
        [HttpPost]
        [Route("LogoutAllServices")]
        public async Task<ApiResponse<bool>> LogoutAllServices()
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.userService.LogoutAllDevices(this.CurrentUserEmail));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Update user information APi.
        /// </summary>
        /// <param name="user">user.</param>
        /// <returns>User Base Information.</returns>
        [HttpPut]
        [Route("UpdateProfile")]
        public async Task<ApiResponse<bool>> UpdateProfile([FromBody] UserDto user)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.userService.UpdateProfile(user, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
