// <copyright file="ICallApiZabbixHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Call Api helper Interface.
    /// </summary>
    public interface ICallApiZabbixHelper
    {
        /// <summary>
        /// GetHostProble.
        /// </summary>
        /// <param name="groupids">groupIds.</param>
        /// <returns>response string.</returns>
        Task<string> GetHostProblem(string groupids);

        /// <summary>
        /// GetTokenZabbix.
        /// </summary>
        /// <returns>response string.</returns>
        Task<string> GetTokenZabbix();

        /// <summary>
        /// AddHostGroup.
        /// </summary>
        /// <param name="groupName">groupName.</param>
        /// <returns>response string.</returns>
        Task<string> AddGroup(string groupName);

        /// <summary>
        /// Update Group.
        /// </summary>
        /// <param name="groupDto">groupDto.</param>
        /// <returns>response string.</returns>
        Task<string> UpdateGroup(GroupDto groupDto);

        /// <summary>
        /// DeleteHostGroup.
        /// </summary>
        /// <param name="ids">ids.</param>
        /// <returns>response string.</returns>
        Task<string> DeleteGroup(List<int> ids);

        /// <summary>
        /// AddHostToGroup.
        /// </summary>
        /// <param name="hostIds">hostIds.</param>
        /// <param name="groupId">groupId.</param>
        /// <returns>response string.</returns>
        Task<string> AddHostToGroup(List<string> hostIds, string groupId);

        /// <summary>
        /// RemoveHostFromGroup.
        /// </summary>
        /// <param name="hostIds">hostIds.</param>
        /// <param name="groupId">groupId.</param>
        /// <returns>response string.</returns>
        Task<string> RemoveHostFromGroup(List<string> hostIds, List<string> groupId);

        /// <summary>
        /// GetGroupById.
        /// </summary>
        /// <param name="hostIds">hostIds.</param>
        /// <returns>response string.</returns>
        Task<string> GetGroupById(List<int> hostIds);

        /// <summary>
        /// Find group by name.
        /// </summary>
        /// <param name="groupName">group name.</param>
        /// <returns>GroupZabbixDto.</returns>
        Task<GroupZabbixDto> FindGroupByName(string groupName);

        /// <summary>
        /// Get Groups.
        /// </summary>
        /// <returns>result json string.</returns>
        Task<string> GetGroups();

        /// <summary>
        /// FindWorkspaceByName.
        /// </summary>
        /// <param name="workspaceName">workspaceName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<WorkspaceZabbixDto> FindWorkspaceByName(string workspaceName);

        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="workspaceName">workspaceName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> AddWorkspace(string workspaceName);

        /// <summary>
        /// GetHostByGroup.
        /// </summary>
        /// <param name="groupids">groupids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetHostByGroup(string groupids);

        /// <summary>
        /// DeleteHost.
        /// </summary>
        /// <param name="idHost">idHost.</param>
        /// <returns>string.</returns>
        Task<string> DeleteHost(List<string> idHost);

        /// <summary>
        /// GetAllHost.
        /// </summary>
        /// <returns>string.</returns>
        Task<string> GetAllHost();
    }
}
