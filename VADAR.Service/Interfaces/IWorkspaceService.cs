// <copyright file="IWorkspaceService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using VADAR.DTO;
using VADAR.Model.Models;
using VADAR.Service.Common.Interfaces;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// IWorkspace Service.
    /// </summary>
    public interface IWorkspaceService : IEntityService<Workspace>
    {
        /// <summary>
        /// GetAllWorkspace.
        /// </summary>
        /// <param name="workspaceRequestDto">workspaceRequestDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<WorkspaceResultPagingDto> GetAllWorkspace(WorkspacePagingRequestDto workspaceRequestDto);

        /// <summary>
        /// GetWorkspaceById.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<WorkspaceViewModelDto> GetWorkspaceById(int id, string currentUserId);

        /// <summary>
        /// AddWorkspace.
        /// </summary>
        /// <param name="workspaceDto">Workspace Data Transfer Object.</param>
        /// <param name="isSupperAdmin">isSupperAdmin.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AddWorkspace(WorkspaceDto workspaceDto, bool isSupperAdmin = true);

        /// <summary>
        /// UpdateWorkspace.
        /// </summary>
        /// <param name="workspaceDto">workspaceDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateWorkspace(WorkspaceDto workspaceDto);

        /// <summary>
        /// DeleteWorkspace.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> DeleteWorkspace(int id);

        /// <summary>
        /// CheckLicensePreInstall.
        /// </summary>
        /// <param name="tokenWorkspace">tokenWorkspace.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CheckLicensePreInstall(string tokenWorkspace);

        /// <summary>
        /// Delete By List Id.
        /// </summary>
        /// <param name="workspaceIds">workspaceIds.</param>
        /// <returns>bool.</returns>
        Task<bool> DeleteByListId(int[] workspaceIds);

        /// <summary>
        /// Get All Workspace by user Id.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <returns>Workspaces.</returns>
        Task<IEnumerable<WorkspaceDto>> GetAllWorkspaceByUserId(string userId);

        /// <summary>
        /// AutoCreateWorkpsace.
        /// </summary>
        /// <param name="registrationDto">registrationDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> AutoCreateWorkspace(RegistrationDto registrationDto);
    }
}
