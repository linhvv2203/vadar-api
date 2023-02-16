// <copyright file="GroupController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers.BaseControllers
{
    /// <summary>
    /// Initialises a new instance of the <see cref="GroupController"/> class.
    /// </summary>
    public class GroupController : BaseController
    {
        private readonly ILoggerHelper<GroupController> logger;
        private readonly IGroupService groupService;

        /// <summary>
        /// Initialises a new instance of the <see cref="GroupController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="groupService">Group Service.</param>
        public GroupController(ILoggerHelper<GroupController> logger, IGroupService groupService)
        {
            this.logger = logger;
            this.groupService = groupService;
        }

        /// <summary>
        /// GetGroupById.
        /// </summary>
        /// <param name="groupName">groupName.</param>
        /// <returns>GroupDto.</returns>
        [HttpGet("{groupName}")]
        public async Task<ApiResponse<GroupViewModelDto>> GetGroupById(string groupName)
        {
            try
            {
                return new ApiResponse<GroupViewModelDto>(EnApiStatusCode.Success, await this.groupService.GetGroupByName(groupName, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<GroupViewModelDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetAllGroup.
        /// </summary>
        /// <param name="groupRequestDto">groupRequestDto.</param>
        /// <returns>GroupResultPagingDto.</returns>
        [HttpGet]
        public async Task<ApiResponse<GroupResultPagingDto>> GroupPaging([FromQuery] GroupPagingRequestDto groupRequestDto)
        {
            try
            {
                groupRequestDto.CreatedById = this.CurrentUserId;
                return new ApiResponse<GroupResultPagingDto>(EnApiStatusCode.Success, await this.groupService.GetAllGroup(groupRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<GroupResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// AddGroup.
        /// </summary>
        /// <param name="groupDto ">groupDto.</param>
        /// <returns>bool.</returns>
        [HttpPost]
        public async Task<ApiResponse<bool>> AddGroup([FromBody] GroupDto groupDto)
        {
            try
            {
                if (groupDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(groupDto));
                }

                groupDto.CreatedById = this.CurrentUserId;
                groupDto.CreatedDate = DateTime.UtcNow;
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.groupService.AddGroup(groupDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// UpdateGroup.
        /// </summary>
        /// <param name="groupDto">groupDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut]
        public async Task<ApiResponse<bool>> UpdateGroup([FromBody] GroupDto groupDto)
        {
            try
            {
                if (groupDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(groupDto));
                }

                groupDto.UpdatedById = this.CurrentUserId;
                groupDto.UpdatedDate = DateTime.Now;
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.groupService.UpdateGroup(groupDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// DeleteGroup.
        /// </summary>
        /// <param name="groupId">groupId.</param>
        /// <returns>bool.</returns>
        [HttpDelete("{groupId}")]
        public async Task<ApiResponse<bool>> DeleteGroup(Guid groupId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.groupService.DeleteGroup(groupId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// DeleteGroupByIds.
        /// </summary>
        /// <param name="groupIds">groupIds.</param>
        /// <returns>success: true, false: fail.</returns>
        [HttpPost("DeleteGroupByIds")]
        public async Task<ApiResponse<bool>> DeleteGroupByIds([FromBody] Guid[] groupIds)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.groupService.DeleteGroupByIds(groupIds, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
