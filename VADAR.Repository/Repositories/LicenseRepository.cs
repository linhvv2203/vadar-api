// <copyright file="LicenseRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// License Repository.
    /// </summary>
    public class LicenseRepository : GenericRepository<License>, ILicenseRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LicenseRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public LicenseRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
