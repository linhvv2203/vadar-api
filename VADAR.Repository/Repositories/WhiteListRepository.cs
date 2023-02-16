// <copyright file="WhiteListRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// WhiteList Repository.
    /// </summary>
    public class WhiteListRepository : GenericRepository<WhiteIp>, IWhiteListRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WhiteListRepository"/> class.
        /// </summary>
        /// <param name="context">context.</param>
        public WhiteListRepository(IDbContext context)
            : base(context)
        {
        }
    }
}
