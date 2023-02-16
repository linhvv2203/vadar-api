// <copyright file="ICallApiHostWazuhHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Call Api helper Interface.
    /// </summary>
    public interface ICallApiHostWazuhHelper
    {
        /// <summary>
        /// AddHostWazuh.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>response string.</returns>
        Task<string> AddHostWazuh(string name);

        /// <summary>
        /// AddHostWazuh.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>response string.</returns>
        Task<string> RemoveHostWazuh(string name);

        /// <summary>
        /// GetHostById.
        /// </summary>
        /// <param name="hostIdRef">hostIdRef.</param>
        /// <returns>response string.</returns>
        Task<string> GetHostById(string hostIdRef);
    }
}
