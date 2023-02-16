// <copyright file="ICallApiWazuhHelper.cs" company="VSEC">
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
    public interface ICallApiWazuhHelper
    {
        /// <summary>
        /// AddHostWazuh.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>response string.</returns>
        Task<string> AddHostWazuh(string name);

        /// <summary>
        /// AddHostWazuh.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>response string.</returns>
        Task<string> RemoveHostWazuh(string name);

        /// <summary>
        /// AddHostWazuh.
        /// </summary>
        /// <param name="listIds">listIds.</param>
        /// <param name="groupName">groupName.</param>
        /// <returns>response string.</returns>
        Task<string> AddHostToGroupWazuh(List<string> listIds, string groupName);

        /// <summary>
        /// AddHostWazuh.
        /// </summary>
        /// <param name="listIds">listIds.</param>
        /// <param name="groupId">groupId.</param>
        /// <returns>response string.</returns>
        Task<string> RemoveHostFromGroupWazuh(List<string> listIds, string groupId);

        /// <summary>
        /// GetGroups.
        /// </summary>
        /// <returns>string.</returns>
        Task<string> GetGroups();

        /// <summary>
        /// GetAllHost.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> GetAllHost();

        /// <summary>
        /// Get Group Detail.
        /// </summary>
        /// <param name="groupName">group name.</param>
        /// <returns>group Detail json string.</returns>
        Task<GroupDto> GetGroupDetail(string groupName);

        /// <summary>
        /// Create A Group.
        /// </summary>
        /// <param name="groupName">group Name.</param>
        /// <returns>result json.</returns>
        Task<string> CreateAGroup(string groupName);

        /// <summary>
        /// Remove A Group.
        /// </summary>
        /// <param name="groupName">group Name.</param>
        /// <returns>Result Json.</returns>
        Task<string> RemoveAGroup(string groupName);

        /// <summary>
        /// Remove List Of Groups.
        /// </summary>
        /// <param name="groupNames">groupNames.</param>
        /// <returns>{
        /// "error": 0,
        /// "data": {
        /// "msg": "All selected groups were removed",
        /// "ids": [
        /// "webserver",
        /// "database"
        /// ],
        /// "affected_agents": [
        /// "002",
        /// "005"
        /// ]
        /// }
        /// }.</returns>
        Task<string> RemoveListOfGroups(string[] groupNames);

        /// <summary>
        /// GetWorkspaceDetail.
        /// </summary>
        /// <param name="workspaceName">workspaceName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<WorkspaceDto> GetWorkspaceDetail(string workspaceName);

        /// <summary>
        /// CreateAWorkspace.
        /// </summary>
        /// <param name="workspaceName">workspaceName.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> CreateAWorkspace(string workspaceName);

        /// <summary>
        /// AddAHostToGroupWazuh.
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <param name="groupName">groupName.</param>
        /// <returns>string.</returns>
        Task<string> AddAHostToGroupWazuh(string hostId, string groupName);

        /// <summary>
        /// GetAllHostByGroup.
        /// </summary>
        /// <param name="groupId">groupId.</param>
        /// <returns>string.</returns>
        Task<string> GetAllHostByGroup(string groupId);
    }
}
