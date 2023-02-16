// <copyright file="RedisCachingHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Redis Caching Helper.
    /// </summary>
    public class RedisCachingHelper : IRedisCachingHelper
    {
        private readonly IDistributedCache distributedCache;
        private readonly string prefix = "vsecwebsite_";
        private ILoggerHelper<RedisCachingHelper> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="RedisCachingHelper"/> class.
        /// Redis Caching Helper.
        /// </summary>
        /// <param name="cache">cache.</param>
        /// <param name="logger">Logger.</param>
        public RedisCachingHelper(IDistributedCache cache, ILoggerHelper<RedisCachingHelper> logger)
        {
            this.distributedCache = cache;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<string> GetDataByKey(string key)
        {
            try
            {
                return await this.distributedCache.GetStringAsync(this.prefix + key);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public async Task RemoveByKey(string key)
        {
            try
            {
                await this.distributedCache.RemoveAsync(this.prefix + key);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
            }
        }

        /// <inheritdoc/>
        public async Task SetObjectData(string key, object data, long seconds = 0)
        {
            try
            {
                if (seconds > 0)
                {
                    var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(seconds));
                    await this.distributedCache.SetStringAsync(this.prefix + key, JsonConvert.SerializeObject(data), options);
                    return;
                }

                await this.distributedCache.SetStringAsync(this.prefix + key, JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
            }
        }

        /// <inheritdoc/>
        public async Task SetStringData(string key, string data, long seconds = 0)
        {
            try
            {
                if (seconds > 0)
                {
                    var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(seconds));
                    await this.distributedCache.SetStringAsync(this.prefix + key, data, options);
                    return;
                }

                await this.distributedCache.SetStringAsync(this.prefix + key, data);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(0), ex, ex.GetBaseException().Message + '\n' + ex.GetBaseException().StackTrace);
            }
        }
    }
}
