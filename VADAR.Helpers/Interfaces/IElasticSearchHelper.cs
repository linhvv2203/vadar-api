// <copyright file="IElasticSearchHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Nest;
using VADAR.Helpers.Models;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// ElasticSearch Helper.
    /// </summary>
    public interface IElasticSearchHelper
    {
        /// <summary>
        /// Indexing a document.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="entity">Entity Object.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> Index<T>(T entity)
            where T : ElasticSearchBaseEntity;

#pragma warning disable SA1618 // Generic type parameters should be documented
        /// <summary>
        /// Get entity entry.
        /// </summary>
        /// <param name="id">Get By Id.</param>
        /// <returns>Entity Entry Instance. </returns>
        Task<T> GetById<T>(string id)
#pragma warning restore SA1618 // Generic type parameters should be documented
            where T : ElasticSearchBaseEntity;

        /// <summary>
        /// Search by query.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="query">Query.</param>
        /// <returns>Search Response.</returns>
        Task<ISearchResponse<T>> SearchByQuery<T>(Func<SearchDescriptor<T>, ISearchRequest> query)
            where T : ElasticSearchBaseEntity;

        /// <summary>
        /// Search by query.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="query">Query string.</param>
        /// <returns>Search Response.</returns>
        Task<ISearchResponse<T>> SearchByQueryString<T>(string query)
            where T : ElasticSearchBaseEntity;

        /// <summary>
        /// Count By Query.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="query">Query.</param>
        /// <returns>Count Response.</returns>
        Task<CountResponse> CountByQuery<T>(Func<CountDescriptor<T>, ICountRequest> query)
            where T : ElasticSearchBaseEntity;
    }
}
