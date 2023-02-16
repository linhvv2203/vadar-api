// <copyright file="PolicyController.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    /// Policy Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : BaseController
    {
        private readonly ILoggerHelper<PolicyController> logger;
        private readonly IPolicyService policyService;

        /// <summary>
        /// Initialises a new instance of the <see cref="PolicyController"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="policyService">Policy Service.</param>
        public PolicyController(
                ILoggerHelper<PolicyController> logger,
                IPolicyService policyService)
        {
            this.logger = logger;
            this.policyService = policyService;
        }

        /// <summary>
        /// Get Logs performance Paging.
        /// </summary>
        /// <param name="machineId">Machine Id.</param>
        /// <returns>LogsPerformanceResultPaging.</returns>
        [HttpGet]
        [Route("GetPoliciesAndWhiteList/{machineId}")]
        [AllowAnonymous]
        public async Task<ApiResponse<PolicyAndWhiteListIpResultDto>> GetPoliciesAndWhiteList(string machineId)
        {
            try
            {
                return new ApiResponse<PolicyAndWhiteListIpResultDto>(EnApiStatusCode.Success, await this.policyService.GetPoliciesAndWhiteList(machineId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<PolicyAndWhiteListIpResultDto>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Policies Paging.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="desciption">desciption.</param>
        /// <returns>Policies Result.</returns>
        [HttpGet]
        [Route("GetPolicies/{workspaceId}")]
        public async Task<ApiResponse<IEnumerable<PolicyViewDto>>> GetPolicies(int workspaceId, string desciption)
        {
            try
            {
                return new ApiResponse<IEnumerable<PolicyViewDto>>(EnApiStatusCode.Success, await this.policyService.GetPolicies(workspaceId, desciption, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<IEnumerable<PolicyViewDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Policies Paging.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="ip">ip.</param>
        /// <returns>Policies Result.</returns>
        [HttpGet]
        [Route("GetWhiteList/{workspaceId}")]
        public async Task<ApiResponse<IEnumerable<IpDto>>> GetWhiteList(int workspaceId, string ip)
        {
            try
            {
                return new ApiResponse<IEnumerable<IpDto>>(EnApiStatusCode.Success, await this.policyService.GetWhiteIps(workspaceId, ip, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<IEnumerable<IpDto>>(ex.HResult);
            }
        }

        /// <summary>
        /// Get Policies Paging.
        /// </summary>
        /// <param name="policiesPagingRequestDto">policiesPagingRequestDto.</param>
        /// <returns>PolicyResultPagingDto.</returns>
        [HttpGet]
        [Route("GetPoliciesPaging")]
        public async Task<ApiResponse<PolicyResultPagingDto>> GetPoliciesPaging([FromQuery]PoliciesPagingRequestDto policiesPagingRequestDto)
        {
            try
            {
                return new ApiResponse<PolicyResultPagingDto>(EnApiStatusCode.Success, await this.policyService.GetPoliciesPaging(policiesPagingRequestDto, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<PolicyResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// UpdatePolicies.
        /// </summary>
        /// <param name="updatePoliciesRequestDto">updatePoliciesRequestDto.</param>
        /// <returns>true: success, false: fail.</returns>
        [HttpPut]
        public async Task<ApiResponse<bool>> UpdatePolicies([FromBody]UpdatePoliciesRequestDto updatePoliciesRequestDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.policyService.UpdatePolicies(updatePoliciesRequestDto, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// GetWhiteIpPaging.
        /// </summary>
        /// <param name="whiteIpPagingRequestDto">whiteIpPagingRequestDto.</param>
        /// <returns>WhiteIpResultPagingDto.</returns>
        [HttpGet]
        [Route("GetWhiteIpPaging")]
        public async Task<ApiResponse<WhiteIpResultPagingDto>> GetWhiteIpPaging([FromQuery]WhiteIpPagingRequestDto whiteIpPagingRequestDto)
        {
            try
            {
                return new ApiResponse<WhiteIpResultPagingDto>(EnApiStatusCode.Success, await this.policyService.GetWhiteIpPaging(whiteIpPagingRequestDto, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<WhiteIpResultPagingDto>(ex.HResult);
            }
        }

        /// <summary>
        /// CreateWhiteIp.
        /// </summary>
        /// <param name="createWhiteIpDto">createWhiteIpDto.</param>
        /// <returns>true: success, false: fail.</returns>
        [HttpPost]
        [Route("CreateWhiteIp")]
        public async Task<ApiResponse<bool>> CreateWhiteIp([FromBody]CreateWhiteIpDto createWhiteIpDto)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.policyService.CreateWhiteIp(createWhiteIpDto, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// DeleteWhiteIp.
        /// </summary>
        /// <param name="ip">ip.</param>
        /// <returns>true: success, false: fail.</returns>
        [HttpDelete]
        [Route("DeleteWhiteIp/{ip}")]
        public async Task<ApiResponse<bool>> DeleteWhiteIp(string ip)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.policyService.DeleteWhiteIp(ip, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }

        /// <summary>
        /// DeleteWhiteIp.
        /// </summary>
        /// <param name="ipDtos">ipDtos.</param>
        /// <returns>true: success, false: fail.</returns>
        [HttpPost]
        [Route("DeleteWhiteIps")]
        public async Task<ApiResponse<bool>> DeleteWhiteIps([FromBody] IEnumerable<IpDto> ipDtos)
        {
            try
            {
                return new ApiResponse<bool>(EnApiStatusCode.Success, await this.policyService.DeleteWhiteIps(ipDtos, this.CurrentUserId));
            }
            catch (VadarException ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
                return new ApiResponse<bool>(ex.HResult);
            }
        }
    }
}
