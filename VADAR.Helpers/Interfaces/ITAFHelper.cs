// <copyright file="ITAFHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Two Authentication Factor helper interface.
    /// </summary>
    public interface ITAFHelper
    {
        /// <summary>
        /// Verify 2af code.
        /// </summary>
        /// <param name="verificationCode">verification code.</param>
        /// <param name="token">access token.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> VerifyCode(string verificationCode, string token);

        /// <summary>
        /// Update Avatar.
        /// </summary>
        /// <param name="token">access token.</param>
        /// <param name="avatar">avatar.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> UpdateAvatar(string token, string avatar);
    }
}
