// <copyright file="IHostService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Host Service Interface.
    /// </summary>
    public interface IHostService
    {
        /// <summary>
        /// GetHostByName.
        /// </summary>
        /// <param name="hostName">hostName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HostViewModelDto> GetHostByName(string hostName);

        /// <summary>
        /// GetHostById.
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HostViewModelDto> GetHostById(Guid hostId);

        /// <summary>
        /// GetAllHostEngine.
        /// </summary>
        /// <param name="type">type.</param>
        /// <param name="tokenWorkspace">tokenWorkspace.</param>
        /// <param name="module">module.</param>
        /// <param name="currentId">currentId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<dynamic> GetAllHostEngine(int type, int tokenWorkspace, string module, string currentId);

        /// <summary>
        /// GetAllHost.
        /// </summary>
        /// <param name="hostPagingRequestDto">hostPagingRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HostResultPagingDto> GetAllHost(HostPagingRequestDto hostPagingRequestDto, string currentUserId);

        /// <summary>
        /// AddHost.
        /// </summary>
        /// <param name="hostDto">hostDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AddHost(HostDto hostDto);

        /// <summary>
        /// UpdateHost.
        /// </summary>
        /// <param name="hostDto">hostDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateHost(HostDto hostDto);

        /// <summary>
        /// AddHostEngine.
        /// </summary>
        /// <param name="hostName">hostName.</param>
        /// <param name="os">os.</param>
        /// <param name="ip">ip.</param>
        /// <returns>true false.</returns>
        Task<bool> AddHostEngine(string hostName, string os, string ip);

        /// <summary>
        /// GetHostNotExistGroup.
        /// </summary>
        /// <param name="hostPagingRequestDto">hostPagingRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HostResultPagingDto> GetHostNotExistGroup(HostPagingRequestDto hostPagingRequestDto, string currentUserId);

        /// <summary>
        /// DeleteHost .
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>true | false.</returns>
        Task<bool> DeleteHost(Guid hostId, int workspaceId, string currentUserId);

        /// <summary>
        /// CheckHostStatus.
        /// </summary>
        /// <param name="wazuhRef">wazuhRef.</param>
        /// <param name="zabbixRef">zabbixRef.</param>
        /// <param name="hostViews">hostViews.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CheckHostStatus(string wazuhRef, string zabbixRef, List<HostViewModelDto> hostViews);
    }
}
