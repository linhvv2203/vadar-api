// <copyright file="IIDSHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Two Authentication Factor helper interface.
    /// </summary>
    public interface IIDSHelper
    {
        /// <summary>
        /// Verify 2af code.
        /// </summary>
        /// <param name="verificationCode">verification code.</param>
        /// <param name="token">access token.</param>
        /// <returns>true: success; false: failed.</returns>
        Task<bool> VerifyCode(string verificationCode, string token);
    }
}
