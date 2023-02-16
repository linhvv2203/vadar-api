// <copyright file="AgentInstallService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Model.Models;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Common;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Alert Service.
    /// </summary>
    public class AgentInstallService : EntityService<AgentInstall>, IAgentInstallService
    {
        private readonly IAgentInstallUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        /// <summary>
        /// Initialises a new instance of the <see cref="AgentInstallService"/> class.
        /// </summary>
        /// <param name="unitOfWork">unitOfWork.</param>
        /// <param name="mapper">mapper.</param>
        public AgentInstallService(
            IAgentInstallUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, unitOfWork.AgentInstallRepository)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AgentOsDto>> GetListAgentIntallByWorkspace(int workspaceId, string currentUserId)
        {
            if (workspaceId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentNull, nameof(workspaceId));
            }

            if (!await this.ValidatePermission(currentUserId, workspaceId, new[] { (long)EnPermissions.HostView, (long)EnPermissions.HostSetting, (long)EnPermissions.FullPermission }, this.unitOfWork.RolePermissionRepository, this.unitOfWork.WorkspaceRolePermissionRepository))
            {
                throw new VadarException(ErrorCode.Forbidden);
            }

            var results = (await this.unitOfWork.AgentOsRepository.FindBy(i => i.WorkspaceId == workspaceId))
                .Include(i => i.AgentInstalls)
                .Include(i => i.Workspace)
                .Select(s => this.mapper.Map<AgentOsDto>(s));

            return results;
        }
    }
}
