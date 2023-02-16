// <copyright file="IWorkspaceClaimRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VADAR.Model;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// WorkspaceClaim Repository interface.
    /// </summary>
    public interface IWorkspaceClaimRepository : IGenericRepository<WorkspaceClaim>
    {
    }
}
