// <copyright file="RoleService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Role Service.
    /// </summary>
    public class RoleService : EntityService<Role>, IRoleService
    {
        private readonly IRoleUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        /// <summary>
        /// Initialises a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="mapper">mapper.</param>
        public RoleService(IRoleUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, unitOfWork.RoleRepository)
        {
            Guard.IsNotNull(mapper, nameof(mapper));
            Guard.IsNotNull(unitOfWork, nameof(unitOfWork));

            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<bool> AddRole(RoleInputDto dto, string currentUserId)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNullOrEmpty);
            }

            if (!dto.PermissionIds.Any())
            {
                throw new VadarException(ErrorCode.PermissionIsRequired);
            }

            var roles = (await this.unitOfWork.RoleRepository.GetAll()).Where(g => g.Name.ToLower() == dto.Name.ToLower());

            if (roles.Any())
            {
                throw new VadarException(ErrorCode.RoleNameExists, nameof(ErrorCode.RoleNameExists));
            }

            var rolePermissions = new List<RolePermission>();

            foreach (var permissionId in dto.PermissionIds)
            {
                var permissions = (await this.unitOfWork.PermissionRepository.GetAll()).Where(x => x.Id == permissionId).ToList();

                rolePermissions.AddRange(permissions.Select(per => new RolePermission { Permission = per }));
            }

            var role = new Role
            {
                Name = dto.Name,
                CreatedById = currentUserId,
                CreatedDate = DateTime.UtcNow,
                RolePermissions = rolePermissions,
            };

            await this.unitOfWork.RoleRepository.Add(role);

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteRole(Guid roleId, string currentUserId)
        {
            if (roleId == Guid.Empty || string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var role = (await this.unitOfWork.RoleRepository.GetAll())
                .Include(rp => rp.RolePermissions).FirstOrDefault(r => r.Id == roleId);

            if (role == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var rolesOfUser = (await this.unitOfWork.RoleUserRepository.GetAll()).FirstOrDefault(x => x.RoleId == roleId);

            if (rolesOfUser != null)
            {
                throw new VadarException(ErrorCode.RoleAlreadyHasUser);
            }

            await this.unitOfWork.RoleRepository.Delete(role);

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RoleViewDto>> GetAllRoles(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            return (await this.unitOfWork.RoleRepository.GetAll())
                .Include(g => g.RolePermissions)
                .Select(r => new RoleViewDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Permissions = this.unitOfWork.RolePermissionRepository.GetAll().Result.Where(x => x.RoleId == r.Id)
                                .Select(rs => this.mapper.Map<PermissionDto>(rs.Permission)),
                    UsersOfRole = this.unitOfWork.RoleUserRepository.GetAll().Result.Where(x => x.RoleId == r.Id).Include(u => u.User)
                                .Select(gu => this.mapper.Map<UserDto>(gu.User)),
                });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PermissionDto>> GetPermissions()
        {
            return (await this.unitOfWork.PermissionRepository.GetAll()).Select(r => this.mapper.Map<PermissionDto>(r));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Guid>> GetRoleIdsByUserId(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            return (await this.unitOfWork.RoleUserRepository.GetAll()).Where(ru => ru.UserId == currentUserId).Select(ru => ru.RoleId);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateRole(RoleInputDto dto, string currentUserId)
        {
            if (dto == null && string.IsNullOrWhiteSpace(currentUserId))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            if (dto != null && string.IsNullOrEmpty(dto.Name))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(dto.Name));
            }

            if (dto?.PermissionIds != null && !dto.PermissionIds.Any())
            {
                throw new VadarException(ErrorCode.PermissionIsRequired);
            }

            var role = (await this.unitOfWork.RoleRepository.GetAll()).FirstOrDefault(g => g.Id == dto.RoleId);

            if (role == null)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid, nameof(role));
            }

            var rolePermissionOlds = (await this.unitOfWork.RolePermissionRepository.GetAll()).Where(x => x.RoleId == role.Id);

            foreach (var rp in rolePermissionOlds)
            {
                await this.unitOfWork.RolePermissionRepository.Delete(rp);
            }

            if (dto != null && dto.PermissionIds!.Any())
            {
                var permissions = (await this.unitOfWork.PermissionRepository.GetAll()).Where(x => dto.PermissionIds.Any(p => p == x.Id)).ToList();

                var rolePermissions = permissions.Select(permission => new RolePermission { Permission = permission, RoleId = role.Id }).ToList();

                role.RolePermissions = rolePermissions;
            }

            role.Name = dto?.Name;
            role.UpdateById = currentUserId;
            role.UpdatedDate = DateTime.UtcNow;

            await this.unitOfWork.RoleRepository.Edit(role);

            return await this.unitOfWork.Commit() > 0;
        }

        /// <inheritdoc/>
        public async Task<long[]> GetPermissionIdsByUserId(string userId)
        {
            return await this.unitOfWork.RolePermissionRepository.GetPermissionIdsByUserId(userId);
        }
    }
}
