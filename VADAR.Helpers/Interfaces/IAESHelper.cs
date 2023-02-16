// <copyright file="IAESHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// IAESHelper.
    /// </summary>
    public interface IAESHelper
    {
        /// <summary>
        /// EncryptString.
        /// </summary>
        /// <param name="text">text.</param>
        /// <returns>string.</returns>
        string EncryptString(string text);

        /// <summary>
        /// DecryptString.
        /// </summary>
        /// <param name="cipherText">cipherText.</param>
        /// <returns>string.</returns>
        string DecryptString(string cipherText);

        /// <summary>
        /// GetMd5Hash.
        /// </summary>
        /// <param name="input">input.</param>
        /// <returns>string.</returns>
        string GetMd5Hash(string input);
    }
}
