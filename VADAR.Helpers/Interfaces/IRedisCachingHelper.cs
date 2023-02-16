// <copyright file="IRedisCachingHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Redis caching helper interface.
    /// </summary>
    public interface IRedisCachingHelper
    {
        /// <summary>
        /// Get Data By Key.
        /// </summary>
        /// <param name="key">key.</param>
        /// <returns>string data.</returns>
        Task<string> GetDataByKey(string key);

        /// <summary>
        /// Set object data.
        /// </summary>
        /// <param name="key">key.</param>
        /// <param name="data">object data.</param>
        /// <param name="seconds">caching time by seconds. Default = 0, forever.</param>
        /// <returns>Task.</returns>
        Task SetObjectData(string key, dynamic data, long seconds = 0);

        /// <summary>
        /// Set string data.
        /// </summary>
        /// <param name="key">key.</param>
        /// <param name="data">string data.</param>
        /// <param name="seconds">caching time by seconds. Default = 0, forever.</param>
        /// <returns>Task.</returns>
        Task SetStringData(string key, string data, long seconds = 0);

        /// <summary>
        /// Remove By Key.
        /// </summary>
        /// <param name="key">key.</param>
        /// <returns>Task.</returns>
        Task RemoveByKey(string key);
    }
}
