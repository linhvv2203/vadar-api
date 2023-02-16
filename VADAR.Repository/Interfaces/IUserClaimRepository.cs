// <copyright file="IUserClaimRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// User Repository Interface.
    /// </summary>
    public interface IUserClaimRepository : IGenericRepository<UserClaim>
    {
    }
}
