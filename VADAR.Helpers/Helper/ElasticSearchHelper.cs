// <copyright file="ElasticSearchHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nest;
using VADAR.Helpers.Interfaces;
using VADAR.Helpers.Models;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Elastic Search Helper Class.
    /// </summary>
    public class ElasticSearchHelper : IElasticSearchHelper
    {
        private readonly IConfiguration configs;
        private readonly IElasticClient esClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="ElasticSearchHelper"/> class.
        /// </summary>
        /// <param name="configurations">configurations.</param>
        public ElasticSearchHelper(IConfiguration configurations)
        {
            this.configs = configurations;
            var settings = new ConnectionSettings(new Uri(this.configs["ConnectionStrings:VSECConnection:ElasticUrl"]));
            settings.DefaultIndex(this.configs["ConnectionStrings:VSECConnection:Index"]);
            this.esClient = new ElasticClient(settings);
        }

        /// <inheritdoc/>
        public async Task<T> GetById<T>(string id)
            where T : ElasticSearchBaseEntity
        {
            return (await this.esClient.GetAsync<T>(id)).Source;
        }

        /// <inheritdoc/>
        public async Task<ISearchResponse<T>> SearchByQuery<T>(Func<SearchDescriptor<T>, ISearchRequest> query)
            where T : ElasticSearchBaseEntity
        {
            return await this.esClient.SearchAsync(query);
        }

        /// <inheritdoc/>
        public async Task<CountResponse> CountByQuery<T>(Func<CountDescriptor<T>, ICountRequest> query)
            where T : ElasticSearchBaseEntity
        {
            return await this.esClient.CountAsync(query);
        }

        /// <inheritdoc/>
        public async Task<ISearchResponse<T>> SearchByQueryString<T>(string query)
            where T : ElasticSearchBaseEntity
        {
            return await this.esClient.SearchAsync<T>(new SearchRequest { QueryOnQueryString = query });
        }

        /// <inheritdoc/>
        public async Task<bool> Index<T>(T entity)
            where T : ElasticSearchBaseEntity
        {
            if (entity == null)
            {
                return false;
            }

            var result = await this.esClient.IndexAsync(entity, e => e.Index(this.configs["ConnectionStrings:VSECConnection:Index"]));
            return result.Result == Result.Created || result.Result == Result.Updated;
        }
    }
}
