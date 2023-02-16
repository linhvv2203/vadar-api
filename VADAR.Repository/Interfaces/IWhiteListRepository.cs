// <copyright file="IWhiteListRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;

namespace VADAR.Repository.Interfaces
{
    /// <summary>
    /// White List Repository Interface.
    /// </summary>
    public interface IWhiteListRepository : IGenericRepository<WhiteIp>
    {
    }
}
