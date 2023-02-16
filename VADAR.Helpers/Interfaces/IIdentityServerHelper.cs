// <copyright file="IIdentityServerHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using VADAR.DTO;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// IIdentityServerHelper.
    /// </summary>
    public interface IIdentityServerHelper
    {
        /// <summary>
        /// ActiveAccount.
        /// </summary>
        /// <param name="email">email.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<UserDto> ActiveAccount(string email);

        /// <summary>
        /// VerifyEmailExisting.
        /// </summary>
        /// <param name="email">email.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> VerifyEmailExisting(string email);
    }
}
