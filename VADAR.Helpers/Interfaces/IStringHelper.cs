// <copyright file="IStringHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Interfacr for VerifyBase64StringHelper.
    /// </summary>
    public interface IStringHelper
    {
        /// <summary>
        /// GetHostName.
        /// </summary>
        /// <param name="hostName">hostName.</param>
        /// <returns>host Name.</returns>
        public string GetHostName(dynamic hostName);

        /// <summary>
        /// Validate file type.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="fileTypes">File Types.</param>
        /// <returns>true: valid, false: invalid.</returns>
        bool ValidateFileType(string fileName, List<string> fileTypes);

        /// <summary>
        /// Verify Base64.
        /// </summary>
        /// <param name="base64String">Base64 String.</param>
        /// <returns>True means input is base64.</returns>
        bool IsBase64(string base64String);

        /// <summary>
        /// User Status Name.
        /// </summary>
        /// <param name="status">status.</param>
        /// <param name="language">language.</param>
        /// <returns>User Status name.</returns>
        string UserStatusName(int status, string language);

        /// <summary>
        /// Verify File Type.
        /// </summary>
        /// <param name="base64String">base64String.</param>
        /// <returns>Bool.</returns>
        bool IsVerifyFileType(string base64String);

        /// <summary>
        /// Generate Slug.
        /// </summary>
        /// <param name="phrase">phrase for slug.</param>
        /// <returns>slug.</returns>
        string GenerateSlug(string phrase);

        /// <summary>
        /// IsValidEmail.
        /// </summary>
        /// <param name="email">email.</param>
        /// <returns>bool.</returns>
        bool IsValidEmail(string email);

        /// <summary>
        /// Base64Decode.
        /// </summary>
        /// <param name="base64EncodedData">base64EncodedData.</param>
        /// <returns>T.</returns>
        string Base64Decode(string base64EncodedData);

        /// <summary>
        /// EncodeBase64.
        /// </summary>
        /// <param name="value">value.</param>
        /// <returns>T.</returns>
        string EncodeBase64(string value);

        /// <summary>
        /// RemoveVietnameseTone.
        /// </summary>
        /// <param name="companyName">companyName.</param>
        /// <returns>T.</returns>
        string RemoveVietnameseTone(string companyName);

        /// <summary>
        /// Generate Random String.
        /// </summary>
        /// <returns>Random unique string.</returns>
        string GenerateRandomString();

        /// <summary>
        /// IsValidPhoneNumber.
        /// </summary>
        /// <param name="phoneNumber">phoneNumber.</param>
        /// <returns>bool.</returns>
        bool IsValidPhoneNumber(string phoneNumber);

        /// <summary>
        /// IsValidUrl.
        /// </summary>
        /// <param name="url">url.</param>
        /// <returns>bool.</returns>
        bool IsValidUrl(string url);

        /// <summary>
        /// IsValidTelegram.
        /// </summary>
        /// <param name="token">token.</param>
        /// <returns>bool.</returns>
        bool IsValidTelegram(string token);
    }
}
