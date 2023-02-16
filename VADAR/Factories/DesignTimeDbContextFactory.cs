// <copyright file="DesignTimeDbContextFactory.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VADAR.Model;

namespace VADAR.WebAPI.Factories
{
    /// <summary>
    /// Design Time DB Context Factory.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VADARDbContext>
    {
        /// <summary>
        /// Create Db Context Method.
        /// </summary>
        /// <param name="args">string arguments.</param>
        /// <returns>Database context.</returns>
        public VADARDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<VADARDbContext>();
            builder.UseMySql(configuration.GetConnectionString("VADARConnection"));
            return new VADARDbContext(configuration);
        }
    }
}
