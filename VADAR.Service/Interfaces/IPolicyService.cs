// <copyright file="IPolicyService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Policy Service Interface.
    /// </summary>
    public interface IPolicyService
    {
        /// <summary>
        /// Get Policies and White List.
        /// </summary>
        /// <param name="machineId">Machine Id.</param>
        /// <returns>Policy And White List Ips.</returns>
        Task<PolicyAndWhiteListIpResultDto> GetPoliciesAndWhiteList(string machineId);

        /// <summary>
        /// Get Policies Paging.
        /// </summary>
        /// <param name="policiesPagingRequestDto">policiesPagingRequestDto.</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>PolicyResultPagingDto.</returns>
        Task<PolicyResultPagingDto> GetPoliciesPaging(PoliciesPagingRequestDto policiesPagingRequestDto, string currentUserId);

        /// <summary>
        /// UpdatePolicies.
        /// </summary>
        /// <param name="updatePoliciesRequestDto">updatePoliciesRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success, false: fail.</returns>
        Task<bool> UpdatePolicies(UpdatePoliciesRequestDto updatePoliciesRequestDto, string currentUserId);

        /// <summary>
        /// GetWhiteIpPaging.
        /// </summary>
        /// <param name="whiteIpPagingRequestDto">whiteIpPagingRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>WhiteIpResultPagingDto.</returns>
        Task<WhiteIpResultPagingDto> GetWhiteIpPaging(WhiteIpPagingRequestDto whiteIpPagingRequestDto, string currentUserId);

        /// <summary>
        /// CreateWhiteIp.
        /// </summary>
        /// <param name="createWhiteIpDto">createWhiteIpDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success, false: fail.</returns>
        Task<bool> CreateWhiteIp(CreateWhiteIpDto createWhiteIpDto, string currentUserId);

        /// <summary>
        /// DeleteWhiteIp.
        /// </summary>
        /// <param name="ip">ip.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success, false: fail.</returns>
        Task<bool> DeleteWhiteIp(string ip, string currentUserId);

        /// <summary>
        /// Get Policies.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <param name="desciption">desciption.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>Data Result.</returns>
        Task<IEnumerable<PolicyViewDto>> GetPolicies(int workspaceId, string desciption, string currentUserId);

        /// <summary>
        /// Get White Ips.
        /// </summary>
        /// <param name="workspaceId">Workspace Id.</param>
        /// <param name="ip">ip.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>Data Result.</returns>
        Task<IEnumerable<IpDto>> GetWhiteIps(int workspaceId, string ip, string currentUserId);

        /// <summary>
        /// Delete White Ips.
        /// </summary>
        /// <param name="ipDtos">ipDtos.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> DeleteWhiteIps(IEnumerable<IpDto> ipDtos, string currentUserId);
    }
}
