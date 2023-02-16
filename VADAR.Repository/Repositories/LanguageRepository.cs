// <copyright file="LanguageRepository.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using VADAR.Model.Models;
using VADAR.Repository.Common;
using VADAR.Repository.Interfaces;

namespace VADAR.Repository.Repositories
{
    /// <summary>
    /// Language Repository.
    /// </summary>
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LanguageRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        public LanguageRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
