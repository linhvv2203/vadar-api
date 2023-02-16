// <copyright file="IGroupService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// Group Service Interface.
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// GetGroupById.
        /// </summary>
        /// <param name="groupId">groupId.</param>
        /// <param name="currentUserId">Current User id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<GroupViewModelDto> GetGroupById(Guid groupId, string currentUserId);

        /// <summary>
        /// GetGroupByName.
        /// </summary>
        /// <param name="groupName">groupName.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<GroupViewModelDto> GetGroupByName(string groupName, string currentUserId);

        /// <summary>
        /// GetAllGroup.
        /// </summary>
        /// <param name="groupRequestDto">groupRequestDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<GroupResultPagingDto> GetAllGroup(GroupPagingRequestDto groupRequestDto, string currentUserId);

        /// <summary>
        /// AddGroup.
        /// </summary>
        /// <param name="group">group.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AddGroup(GroupDto group, string currentUserId);

        /// <summary>
        /// UpdateGroup.
        /// </summary>
        /// <param name="groupDto">groupDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateGroup(GroupDto groupDto);

        /// <summary>
        /// DeleteGroup.
        /// </summary>
        /// <param name="groupId">groupId.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteGroup(Guid groupId, string currentUserId);

        /// <summary>
        /// Delete Group By Ids.
        /// </summary>
        /// <param name="groupIds">groupIds.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteGroupByIds(Guid[] groupIds, string currentUserId);
    }
}
