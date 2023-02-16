// <copyright file="IHostGroupManageService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// IHostGroupManage Service.
    /// </summary>
    public interface IHostGroupManageService
    {
        /// <summary>
        /// Add Host Group By Names.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// /// <param name="name">name.</param>
        Task<ResultHostGroupDto> AddHostGroupByNames(string name);

        /// <summary>
        /// Remove Host Group By Name.
        /// </summary>
        /// /// <param name="idGroup">idGroup.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<ResultHostGroupDto> RemoveHostGroupByName(List<int> idGroup);

        /// <summary>
        /// Add Host To Group By Id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="addHostToGroupByIdRequestDto">addHostToGroupByIdRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        Task<bool> AddHostToGroupById(AddHostToGroupByIdRequestDto addHostToGroupByIdRequestDto, string currentUserId);

        /// <summary>
        /// CheckHostAlreadyExistsInGroup.
        /// </summary>
        /// <param name="addHostToGroupByIdRequestDto">addHostToGroupByIdRequestDto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> CheckHostAlreadyExistsInGroup(AddHostToGroupByIdRequestDto addHostToGroupByIdRequestDto);

        /// <summary>
        /// Remove Host From Group.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// /// <param name="id">id.</param>
        /// /// <param name="groupId">groupId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        Task<bool> RemoveHostFromGroup(List<string> id, string groupId, string currentUserId);
    }
}
