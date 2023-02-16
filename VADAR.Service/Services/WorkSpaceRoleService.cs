// <copyright file="WorkSpaceRoleService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VADAR.DTO;
using VADAR.DTO.ViewModels;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Work Space Role Service.
    /// </summary>
    public class WorkSpaceRoleService : EntityService<WorkspaceRole>, IWorkSpaceRoleService
    {
        private readonly IWorkspaceRoleUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkSpaceRoleService"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit Of Work.</param>
        /// <param name="mapper">Mapper.</param>
        public WorkSpaceRoleService(IWorkspaceRoleUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, unitOfWork.WorkspaceRoleRepository)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteById(Guid id, string currentUserId)
        {
            await this.DeleteOneWorkspaceRole(id, currentUserId);
            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteByIds(Guid[] ids, string currentUserId)
        {
            if (ids == null || ids.Length <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            foreach (var id in ids)
            {
                await this.DeleteOneWorkspaceRole(id, currentUserId);
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> AddWorkspaceRole(WorkspaceRoleDto workspaceRoleDto, string currentUserId)
        {
            if (workspaceRoleDto == null || workspaceRoleDto.WorkspaceId <= 0 || string.IsNullOrEmpty(workspaceRoleDto.Name))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(workspaceRoleDto));
            }

            var workspaceRole = (await this.unitOfWork.WorkspaceRoleRepository.GetAll()).FirstOrDefault(g => g.Name == workspaceRoleDto.Name.Trim() && g.WorkspaceId == workspaceRoleDto.WorkspaceId);
            if (workspaceRole != null)
            {
                throw new VadarException(ErrorCode.WorkspaceRoleExists, nameof(ErrorCode.WorkspaceExists));
            }

            workspaceRoleDto.CreatedDate = DateTime.UtcNow;

            if (!await this.ValidatePermission(
                currentUserId,
                workspaceRoleDto.WorkspaceId,
                new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission },
                this.unitOfWork.RolePermissionRepository,
                this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            await this.unitOfWork.WorkspaceRoleRepository.Add(this.mapper.Map<WorkspaceRole>(workspaceRoleDto));

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> AssignRoleToUser(Guid[] roleIds, string currentUserId, string userId)
        {
            if (roleIds == null || roleIds.Length <= 0 || string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(userId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workspace = (await this.unitOfWork.WorkspaceRoleRepository.GetWorkspaceRoleById(roleIds[0]))?.Workspace;

            if (workspace == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(currentUserId, workspace.Id, new[] { (int)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var workspaceIds = (await this.unitOfWork.WorkspaceRoleRepository.GetAll())
                .Where(wpr => roleIds.Contains(wpr.Id)).Select(wpr => wpr.WorkspaceId).Distinct().ToList();

            if (workspaceIds.Count > 1)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            // Remove all existing user workspace roles.
            var existingUserWorkSpaceRoles = (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll()).Where(
                uwpr =>
                uwpr.UserId == userId && uwpr.WorkspaceRole.WorkspaceId == workspace.Id);
            foreach (var el in existingUserWorkSpaceRoles)
            {
                await this.unitOfWork.WorkspaceRoleUserRepository.Delete(el);
            }

            // Add new user workspace roles.
            foreach (var el in roleIds)
            {
                await this.unitOfWork.WorkspaceRoleUserRepository.Add(new WorkspaceRoleUser
                {
                    UserId = userId,
                    WorkspaceRoleId = el,
                });
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WorkspaceRoleDto>> GetWorkspaceRoleByWorkspaceId(int workspaceId, string workspaceRoleName, string userId)
        {
            if (workspaceId <= 0 || string.IsNullOrEmpty(userId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workSpace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(workspaceId);

            if (workSpace == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(userId, workSpace.Id, new[] { (long)EnPermissions.WorkspacePermissionView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var query = await this.unitOfWork.WorkspaceRoleRepository.GetWorkspaceRoleByWorkspaceId(workspaceId);
            if (!string.IsNullOrEmpty(workspaceRoleName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(workspaceRoleName.ToLower().Trim()));
            }

            var result = query
                .Select(w => new WorkspaceRoleDto
                {
                    Id = w.Id,
                    Description = w.Description,
                    Name = w.Name,
                    WorkspaceId = w.WorkspaceId,
                    Permissions = string.Join(", ", w.WorkspaceRolePermissions.Select(s => s.Permission.Name)),
                }).ToList();
            return result;
        }

        /// <inheritdoc/>
        public async Task<RoleResultPagingDto> GetWorkspaceRoleByWorkspaceIdPaging(RolePagingRequestDto rolePagingRequestDto, string userId)
        {
            if (rolePagingRequestDto.WorkspaceId <= 0 || string.IsNullOrEmpty(userId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (!await this.ValidatePermission(userId, rolePagingRequestDto.WorkspaceId, new[] { (long)EnPermissions.WorkspacePermissionView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var workSpace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(rolePagingRequestDto.WorkspaceId);

            if (workSpace == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var query = (await this.unitOfWork.WorkspaceRoleRepository.GetAll())
                .Where(x => x.WorkspaceId == rolePagingRequestDto.WorkspaceId);

            if (!string.IsNullOrEmpty(rolePagingRequestDto.WorkspaceRoleName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(rolePagingRequestDto.WorkspaceRoleName.ToLower().Trim()));
            }

            var result = await query
                .Select(g => new WorkspaceRoleViewDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    WorkspaceId = g.WorkspaceId,
                    Permissions = string.Join(", ", g.WorkspaceRolePermissions.Select(s => s.Permission.Name)),
                    CreatedDate = g.CreatedDate.Value,
                })
                .OrderByDescending(o => o.CreatedDate)
                .Skip(rolePagingRequestDto.PageSize * (rolePagingRequestDto.PageIndex - 1))
                .Take(rolePagingRequestDto.PageSize)
                .ToListAsync();

            return new RoleResultPagingDto
            {
                Count = await query.CountAsync(),
                Items = result,
            };
        }

        /// <inheritdoc/>
        public async Task<PermissionListsDto> GetWorkspacePermissionByWorkspaceRoleIdAnUserId(Guid workspaceRoleId, string currentUserId)
        {
            if (workspaceRoleId == Guid.Empty || string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workSpaceRole = await this.unitOfWork.WorkspaceRoleRepository.GetWorkspaceRoleById(workspaceRoleId);

            if (workSpaceRole == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(currentUserId, workSpaceRole.WorkspaceId, new[] { (long)EnPermissions.WorkspacePermissionView, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var permissionLists = new PermissionListsDto();

            var assignedPermissions = (await this.unitOfWork.WorkspaceRolePermissionRepository
                .GetAll()).Where(wrp => wrp.WorkspaceRoleId == workspaceRoleId).Select(wrp => wrp.Permission).ToList();

            var workspacePermissions =
                (await this.unitOfWork.PermissionRepository.GetAll()).Where(p =>
                    p.PermissionType == (int)EnPermissionType.Workspace).ToList();

            permissionLists.UnAssignedPermissions =
                workspacePermissions.Except(assignedPermissions).Select(p => this.mapper.Map<PermissionDto>(p));
            permissionLists.AssignedPermissions = assignedPermissions.Select(p => this.mapper.Map<PermissionDto>(p));
            return permissionLists;
        }

        /// <inheritdoc/>
        public async Task<bool> AssignWorkspacePermissions(int[] permissionIds, string currentUserId, Guid roleId)
        {
            if (permissionIds == null || string.IsNullOrEmpty(currentUserId) || roleId == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            permissionIds = permissionIds.Distinct().ToArray();
            var workSpaceId = (await this.unitOfWork.WorkspaceRoleRepository.GetWorkspaceRoleById(roleId)).WorkspaceId;

            if (workSpaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(currentUserId, workSpaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var oldPermissions = (await this.unitOfWork.WorkspaceRolePermissionRepository.GetAll()).Where(wru =>
                wru.WorkspaceRoleId == roleId).ToList();

            // Delete old permissions.
            foreach (var el in oldPermissions)
            {
                await this.unitOfWork.WorkspaceRolePermissionRepository.Delete(el);
            }

            // Add new permissions.
            foreach (var el in permissionIds)
            {
                await this.unitOfWork.WorkspaceRolePermissionRepository.Add(new WorkspaceRolePermission
                {
                    PermissionId = el,
                    WorkspaceRoleId = roleId,
                });
            }

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WorkspaceRoleDto>> GetWorkspaceRoleByWorkspaceIdAndUserId(
            int workspaceId,
            string currentUserId,
            string userId)
        {
            if (workspaceId <= 0 || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workspace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(workspaceId);

            if (workspace == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(userId, workspaceId, new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            return (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll())
                .Where(wpr => wpr.UserId == userId && wpr.WorkspaceRole.WorkspaceId == workspaceId)
                .Select(wpr => this.mapper.Map<WorkspaceRoleDto>(wpr.WorkspaceRole));
        }

        /// <inheritdoc/>
        public async Task<bool> AssignUserToWorkspaceRole(string userId, string currentUserId, Guid[] workspaceRoleIds)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentUserId) || workspaceRoleIds == null || workspaceRoleIds.Length <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workspaceId = (await this.unitOfWork.WorkspaceRoleUserRepository.GetAll())
                .Where(wpr => workspaceRoleIds.Any(w => w == wpr.WorkspaceRoleId))
                .Select(wp => wp.WorkspaceRole.WorkspaceId).Distinct();

            if (workspaceId == null || workspaceId.Count() > 1)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId.First(), new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            // Remove old roles
            var oldWorkspaceRoleIds = await this.unitOfWork.WorkspaceRoleUserRepository.FindBy(wpr =>
                wpr.UserId == userId && wpr.WorkspaceRole.WorkspaceId == workspaceId.First());

            foreach (var el in oldWorkspaceRoleIds)
            {
                await this.unitOfWork.WorkspaceRoleUserRepository.Delete(el);
            }

            await this.unitOfWork.Commit();

            // Add new roles
            foreach (var el in workspaceRoleIds)
            {
                await this.unitOfWork.WorkspaceRoleUserRepository.Add(new WorkspaceRoleUser
                {
                    UserId = userId,
                    WorkspaceRoleId = el,
                });
            }

            return await this.unitOfWork.Commit() > 0;
        }

        private async Task DeleteOneWorkspaceRole(Guid id, string currentUserId)
        {
            if (id == Guid.Empty)
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var workspaceRole = (await this.unitOfWork.WorkspaceRoleRepository.FindBy(wr => wr.Id == id))
                .Include(inviteWorkspaceRole => inviteWorkspaceRole.InviteWorkspaceRoles)
                .Include(workspaceRoleUser => workspaceRoleUser.WorkspaceRoleUsers)
                .FirstOrDefault();
            if (workspaceRole == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            if (!await this.ValidatePermission(
                currentUserId,
                workspaceRole.WorkspaceId,
                new[] { (long)EnPermissions.WorkspacePermissionSetting, (long)EnPermissions.FullPermission },
                this.unitOfWork.RolePermissionRepository,
                this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            // Kiểm tra workspace role đã có member ở trạng thái chờ xác nhận và đã xác nhận chưa.
            // Nếu đã có thành viên phải xóa hết đối với thành viên đã xác nhận và Hủy với thành viên chưa xác nhận.
            if (workspaceRole.WorkspaceRoleUsers != null && workspaceRole.WorkspaceRoleUsers.Count > 0)
            {
                throw new VadarException(ErrorCode.WorkspaceRoleHaveMember);
            }

            if (workspaceRole.InviteWorkspaceRoles != null && workspaceRole.InviteWorkspaceRoles.Count > 0)
            {
                throw new VadarException(ErrorCode.WorkspaceRoleHaveMember);
            }

            if (workspaceRole.WorkspaceRolePermissions != null)
            {
                foreach (var p in workspaceRole.WorkspaceRolePermissions)
                {
                    await this.unitOfWork.WorkspaceRolePermissionRepository.Delete(p);
                }
            }

            if (workspaceRole.WorkspaceRoleUsers != null)
            {
                foreach (var u in workspaceRole.WorkspaceRoleUsers)
                {
                    await this.unitOfWork.WorkspaceRoleUserRepository.Delete(u);
                }
            }

            await this.unitOfWork.WorkspaceRoleRepository.Delete(workspaceRole);
        }
    }
}
