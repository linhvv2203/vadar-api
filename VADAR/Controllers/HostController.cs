// <copyright file="HostController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Helper;
using VADAR.Service.Interfaces;
using VADAR.WebAPI.Attributes.Filter;
using VADAR.WebAPI.Model;

namespace VADAR.WebAPI.Controllers.BaseControllers
{
    /// <summary>
    /// Initialises a new instance of the <see cref="HostController"/> class.
    /// </summary>
    public class HostController : BaseController
    {
        private readonly ILoggerHelper<HostController> logger;
        private readonly IHostService hostService;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="hostService">hostService.</param>
        public HostController(ILoggerHelper<HostController> logger, IHostService hostService)
        {
            this.logger = logger;
            this.hostService = hostService;
        }

        /// <summary>
        /// GetHostByName.
        /// </summary>
        /// <param name="hostName">hostName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("{hostName}")]
        public async Task<ApiResponse<HostViewModelDto>> GetHostByName(string hostName)
        {
            try
            {
                return new ApiResponse<HostViewModelDto>(EnApiStatusCode.Success, await this.hostService.GetHostByName(hostName));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<HostViewModelDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetHostByName.
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("{hostId}")]
        public async Task<ApiResponse<HostViewModelDto>> GetHostById(Guid hostId)
        {
            try
            {
                return new ApiResponse<HostViewModelDto>(EnApiStatusCode.Success, await this.hostService.GetHostById(hostId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<HostViewModelDto>(ex.HResult);
            }
        }

        /// <summary>
        /// GetAllHostEngine.
        /// </summary>
        /// <param name="type">type.</param>
        /// <param name="tokenWorkspace">tokenWorkspace.</param>
        /// <param name="module">module.</param>
        /// <returns>T.</returns>
        [HttpGet("GetAllHostEngine/{type}/{tokenWorkspace}/{module}")]
        [AllowAnonymous]
        public async Task<dynamic> GetAllHostEngine(int type, int tokenWorkspace, string module)
        {
            try
            {
                return await this.hostService.GetAllHostEngine(type, tokenWorkspace, module, this.CurrentUserId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return ex;
            }
        }

        /// <summary>
        /// GetAllHost.
        /// </summary>
        /// <param name="hostPagingRequestDto">hostPagingRequestDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        public async Task<ApiResponse<HostResultPagingDto>> GetAllHost([FromQuery] HostPagingRequestDto hostPagingRequestDto)
        {
            try
            {
                // hostPagingRequestDto.CreatedById = this.CurrentUserId;
                return new ApiResponse<HostResultPagingDto>(EnApiStatusCode.Success, await this.hostService.GetAllHost(hostPagingRequestDto, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<HostResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// AddHost.
        /// </summary>
        /// <param name="hostDto">hostDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<bool>> AddHost([FromBody] HostDto hostDto)
        {
            try
            {
                if (hostDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(hostDto));
                }

                this.logger.LogInfo(Newtonsoft.Json.JsonConvert.SerializeObject(hostDto));
                hostDto.CreatedById = this.CurrentUserId;
                hostDto.CreatedDate = DateTime.UtcNow;
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.hostService.AddHost(hostDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// UpdateHost.
        /// </summary>
        /// <param name="hostDto">hostDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut]
        public async Task<ApiResponse<bool>> UpdateHost([FromBody] HostDto hostDto)
        {
            try
            {
                if (hostDto is null)
                {
                    throw new VadarException(ErrorCode.ArgumentInvalid, nameof(hostDto));
                }

                hostDto.UpdatedById = this.CurrentUserId;
                hostDto.UpdatedDate = DateTime.Now;
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.hostService.UpdateHost(hostDto));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// UpdateHost.
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <param name="workspaceId">workspaceId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("DeleteHost/{hostId}/{workspaceId}")]
        public async Task<ApiResponse<bool>> DeleteHost(Guid hostId, int workspaceId)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.hostService.DeleteHost(hostId, workspaceId, this.CurrentUserId));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
