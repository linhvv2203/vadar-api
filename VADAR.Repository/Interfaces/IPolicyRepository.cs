// <copyright file="IPolicyRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// Policy Repository.
    /// </summary>
    public interface IPolicyRepository : IGenericRepository<Policy>
    {
    }
}
