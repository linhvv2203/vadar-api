// <copyright file="UserService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// User service class.
    /// </summary>
    public class UserService : EntityService<User>, IUserService
    {
        private readonly IUserUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IGrafanaHelper grafanaHelper;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="mapper">mapper.</param>
        /// <param name="grafanaHelper">Grafana Helper.</param>
        /// <param name="unitOfWork">unitOfWork.</param>
        public UserService(
                        IMapper mapper,
                        IGrafanaHelper grafanaHelper,
                        IUserUnitOfWork unitOfWork)
            : base(unitOfWork, unitOfWork.UserRepository)
        {
            Guard.IsNotNull(unitOfWork, nameof(unitOfWork));
            Guard.IsNotNull(mapper, nameof(mapper));

            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.grafanaHelper = grafanaHelper;
        }

        /// <inheritdoc/>
        public async Task<UserDto> AddUserIfNotExist(UserDto user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Id))
                {
                    return null;
                }

                var userCreated = await this.unitOfWork.UserRepository.GetUserById(user.Id);

                if (userCreated != null)
                {
                    return this.mapper.Map<UserDto>(userCreated);
                }

                user.JoinDate = DateTime.UtcNow;
                userCreated = await this.unitOfWork.UserRepository.Add(this.mapper.Map<User>(user));
                await this.grafanaHelper.CreateGrafanaAccount(new GrafanaAccountDto
                {
                    Email = user.Email,
                    Name = string.IsNullOrEmpty(user.FullName) ? user.Email : user.FullName,
                    Login = user.Email,
                    Password = Guid.NewGuid().ToString(),
                });

                _ = await this.unitOfWork.Commit();

                return (userCreated != null) ? this.mapper.Map<UserDto>(userCreated) : null;
            }
            catch (VadarException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetProfile(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new VadarException(ErrorCode.UserNotFound);
            }

            var userDb = await this.unitOfWork.UserRepository.GetUserById(userId);

            if (userDb == null)
            {
                throw new VadarException(ErrorCode.UserNotFound);
            }

            var user = this.mapper.Map<UserDto>(userDb);
            user.PermissionIds = await this.unitOfWork.RolePermissionRepository.GetPermissionIdsByUserId(userId);

            return user;
        }

        /// <inheritdoc/>
        public async Task<UsersViewPagingDto> GetUsers(UserQueryConditionsDto userQueryConditionsDto, string currentUserId)
        {
            if (userQueryConditionsDto is null || string.IsNullOrWhiteSpace(currentUserId) || userQueryConditionsDto.PageSize <= 0 || userQueryConditionsDto.PageIndex <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var users = await this.unitOfWork.UserRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(userQueryConditionsDto.UserName))
            {
                users = users.Where(u => u.UserName.Contains(userQueryConditionsDto.UserName));
            }

            var currentIndex = (userQueryConditionsDto.PageIndex - 1) * userQueryConditionsDto.PageSize;

            var result = users.OrderByDescending(o => o.JoinDate).Skip(currentIndex).Take(userQueryConditionsDto.PageSize).Select(r => new UserDto
            {
                Id = r.Id,
                Avatar = r.Avatar,
                Email = r.Email,
                UserName = r.UserName,
                RoleId = r.RoleUsers.Any() ? r.RoleUsers.FirstOrDefault().RoleId : Guid.Empty,
            });

            return new UsersViewPagingDto
            {
                TotalUser = users.Count(),
                Users = result,
            };
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateRoleForUser(RoleUserDto roleUserDto, string currentUserId)
        {
            if (roleUserDto == null || string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNullOrWhiteSpace);
            }

            if (!await this.ValidatePermission(currentUserId, null, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, null))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var roleOlds = (await this.unitOfWork.RoleUserRepository.GetAll()).Where(c => c.UserId == roleUserDto.UserId);

            foreach (var role in roleOlds)
            {
                await this.unitOfWork.RoleUserRepository.Delete(role);
            }

            await this.unitOfWork.RoleUserRepository.Add(this.mapper.Map<RoleUser>(roleUserDto));

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<UserBaseInfoDto> GetUserBaseInformation(int? workspaceId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var result = (from u in (await this.unitOfWork.UserRepository.GetAll()).Where(x => x.Id.Trim().ToLower().Equals(userId.Trim().ToLower()))
                          join ur in await this.unitOfWork.RoleUserRepository.GetAll() on u.Id equals ur.UserId into urlj
                          from ur in urlj.DefaultIfEmpty()
                          select new UserBaseInfoDto
                          {
                              Email = u.Email,
                              Id = u.Id,
                              FullName = u.FullName,
                              UserName = u.UserName,
                              Avatar = u.Avatar,
                              RoleId = this.unitOfWork.RoleUserRepository.GetAll().Result.Where(r => r.UserId.Equals(ur.UserId)).Select(r => r.RoleId).ToArray(),
                          }).FirstOrDefault();

            if (result != null)
            {
                result.SystemPermissions =
                    (await this.GetPermissions(userId, workspaceId, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository)).ToArray();
            }

            var claim = await this.unitOfWork.UserClaimsRepository.FindBy(c => c.UserId == userId);
            if (result == null)
            {
                return null;
            }

            result.UserClaims = this.mapper.Map<List<UserClaimDto>>(claim);

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> LogoutAllDevices(string currentEmail)
        {
            if (string.IsNullOrEmpty(currentEmail))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var grafanaAccount = await this.grafanaHelper.GetGrafanaAccountDetail(currentEmail);
            if (grafanaAccount == null || grafanaAccount.Id <= 0)
            {
                return true;
            }

            await this.grafanaHelper.LogoutAllDevices(grafanaAccount.Id);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProfile(UserDto user, string currentUserId)
        {
            if (user is null || string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNullOrWhiteSpace);
            }

            // update user
            var userCreated = await this.unitOfWork.UserRepository.GetUserById(currentUserId);

            if (userCreated is null)
            {
                return false;
            }

            // delete old claim
            var claimOlds = await this.unitOfWork.UserClaimsRepository.FindBy(c => c.UserId == currentUserId);

            foreach (var claim in claimOlds)
            {
                await this.unitOfWork.UserClaimsRepository.Delete(claim);
            }

            // để sau
            // userCreated.FullName = user.FullName;
            userCreated.UserClaims = this.mapper.Map<List<UserClaim>>(user.UserClaims);
            await this.unitOfWork.UserRepository.Edit(userCreated);
            return await this.unitOfWork.Commit() > 0;
        }
    }
}
