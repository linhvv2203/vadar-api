// <copyright file="ICallApiHostZabbixHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Call Api helper Interface.
    /// </summary>
    public interface ICallApiHostZabbixHelper
    {
        /// <summary>
        /// GetTokenZabbix.
        /// </summary>
        /// <returns>response string.</returns>
        Task<string> GetTokenZabbix();

        /// <summary>
        /// AddHost.
        /// </summary>
        /// <param name="hostName">hostName.</param>
        /// <param name="groupId">groupId.</param>
        /// <returns>response string.</returns>
        Task<string> AddHost(string hostName, string groupId);

        /// <summary>
        /// FindHostByHostId.
        /// </summary>
        /// <param name="hostId">hostId.</param>
        /// <returns>response string.</returns>
        Task<string> FindHostByHostId(string hostId);
    }
}
