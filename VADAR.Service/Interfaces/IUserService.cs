// <copyright file="IUserService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Service.Interfaces
{
    /// <summary>
    /// User Service Interface.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Add new user if it is not exist.
        /// </summary>
        /// <param name="user">User information.</param>
        /// <returns>User Created.</returns>
        Task<UserDto> AddUserIfNotExist(UserDto user);

        /// <summary>
        /// Get Profile user.
        /// </summary>
        /// <param name="userId">user Id.</param>
        /// <returns>User Dto.</returns>
        Task<UserDto> GetProfile(string userId);

        /// <summary>
        /// Get Users.
        /// </summary>
        /// <param name="userQueryConditionsDto">userQueryConditionsDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>UsersViewPagingDto.</returns>
        Task<UsersViewPagingDto> GetUsers(UserQueryConditionsDto userQueryConditionsDto, string currentUserId);

        /// <summary>
        /// Update Role For User.
        /// </summary>
        /// <param name="roleUserDto">roleUserDto.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>return True: Success, False: Fail.</returns>
        Task<bool> UpdateRoleForUser(RoleUserDto roleUserDto, string currentUserId);

        /// <summary>
        /// Get user base information.
        /// </summary>
        /// <param name="workspaceId">workspaceId.</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Base User Information.</returns>
        Task<UserBaseInfoDto> GetUserBaseInformation(int? workspaceId, string userId);

        /// <summary>
        /// Logout All Devices.
        /// </summary>
        /// <param name="currentEmail">Current Email.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> LogoutAllDevices(string currentEmail);

        /// <summary>
        /// Update Infor For Expert.
        /// </summary>
        /// <param name="user">user.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <returns>return True: Success, False: Fail.</returns>
        Task<bool> UpdateProfile(UserDto user, string currentUserId);
    }
}
