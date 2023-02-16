// <copyright file="InviteWorkspaceRoleRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// InviteWorkspaceRole Repository.
    /// </summary>
    public class InviteWorkspaceRoleRepository : GenericRepository<InviteWorkspaceRole>, IInviteWorkspaceRoleRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InviteWorkspaceRoleRepository"/> class.
        /// </summary>
        /// <param name="context">content.</param>
        public InviteWorkspaceRoleRepository(IDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<InviteWorkspaceRole> GetInviteWorkspaceRoleById(Guid invitationId)
        {
            return await this.dbset.Where(x => x.Id.Equals(invitationId)).FirstOrDefaultAsync();
        }
    }
}
