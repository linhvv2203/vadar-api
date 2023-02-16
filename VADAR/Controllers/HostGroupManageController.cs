// <copyright file="HostGroupManageController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Controllers.BaseControllers;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers
{
    /// <summary>
    /// HostGroupManage Controller.
    /// </summary>
    public class HostGroupManageController : BaseController
    {
        private readonly ILoggerHelper<HostGroupManageController> logger;
        private readonly IHostGroupManageService hostGroupManageService;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostGroupManageController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="logsService">logsService.</param>
        /// <param name="hostGroupManageService">hostGroupManageService.</param>
        public HostGroupManageController(
            ILoggerHelper<HostGroupManageController> logger,
            ILogsService logsService,
            IHostGroupManageService hostGroupManageService)
        {
            this.logger = logger;
            this.hostGroupManageService = hostGroupManageService;
        }

        /// <summary>
        /// Add Host To Group By Id.
        /// </summary>
        /// <param name="addHostToGroupByIdRequestDto">addHostToGroupByIdRequestDto.</param>
        /// <returns>AddHostToGroupById.</returns>
        [HttpPost]
        [Route("AddHostToGroupById")]
        public async Task<ApiResponse<bool>> AddHostToGroupById([FromBody] AddHostToGroupByIdRequestDto addHostToGroupByIdRequestDto)
        {
            try
            {
                var addHostToGroup = await this.hostGroupManageService.AddHostToGroupById(addHostToGroupByIdRequestDto, this.CurrentUserId);
                return new ApiResponse<bool>(EnApiStatusCode.Success, addHostToGroup);
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// CheckHostAlreadyExistsInGroup.
        /// </summary>
        /// <param name="addHostToGroupByIdRequestDto">addHostToGroupByIdRequestDto.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        [Route("CheckHostAlreadyExistsInGroup")]
        public async Task<ApiResponse<bool>> CheckHostAlreadyExistsInGroup([FromBody] AddHostToGroupByIdRequestDto addHostToGroupByIdRequestDto)
        {
            try
            {
                var addHostToGroup = await this.hostGroupManageService.CheckHostAlreadyExistsInGroup(addHostToGroupByIdRequestDto);
                return new ApiResponse<bool>(EnApiStatusCode.Success, addHostToGroup);
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// Remove Host From Group.
        /// </summary>
        /// <param name="groupHostRemoveRequestDto">groupHostRemoveRequestDto.</param>
        /// <returns>RemoveHostFromGroup.</returns>
        [HttpPost]
        [Route("RemoveHostFromGroup")]
        public async Task<ApiResponse<bool>> RemoveHostFromGroup([FromBody] GroupHostRemoveRequestDto groupHostRemoveRequestDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.hostGroupManageService.RemoveHostFromGroup(groupHostRemoveRequestDto.Id, groupHostRemoveRequestDto.GroupId, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
