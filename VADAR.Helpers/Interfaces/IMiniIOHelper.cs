// <copyright file="IMiniIOHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.IO;
using System.Threading.Tasks;

namespace VADAR.Helpers.Interfaces
{
    /// <summary>
    /// Amazone S3 Helper Interface.
    /// </summary>
    public interface IMiniIOHelper
    {
        /// <summary>
        /// Get File.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>File Stream.</returns>
        Task<Stream> GetFile(string fileName, bool privateBucket = false);

        /// <summary>
        /// Get Avatar Of User.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>url of avatar.</returns>
        string GetAvatarOfUser(string fileName, bool privateBucket = false);

        /// <summary>
        /// Upload File.
        /// </summary>
        /// <param name="base64Content">Base 64 content.</param>
        /// <param name="fileName">File Name.</param>
        /// <param name="contentType">Content Type.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>Url.</returns>
        Task<string> UploadFile(string base64Content, string fileName, string contentType, bool privateBucket = false);

        /// <summary>
        /// Upload File.
        /// </summary>
        /// <param name="contentBytes">byte array content.</param>
        /// <param name="fileName">File Name.</param>
        /// <param name="contentType">Content Type.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <param name="isCensorshipOrganizationFile">isCensorshipOrganizationFile.</param>
        /// <returns>Url.</returns>
        Task<string> UploadFile(byte[] contentBytes, string fileName, string contentType, bool privateBucket = false, bool isCensorshipOrganizationFile = false);

        /// <summary>
        /// Delete File.
        /// </summary>
        /// <param name="fileUrl">File Url.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteFile(string fileUrl, bool privateBucket = false);

        /// <summary>
        /// Get File Size.
        /// </summary>
        /// <param name="fileName">fileName.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>Size File.</returns>
        Task<long> GetSizeFile(string fileName, bool privateBucket = false);

        /// <summary>
        /// Get url for 13 hour download.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>url of file.</returns>
        string GetUrlFor13Hour(string fileName, bool privateBucket = false);

        /// <summary>
        /// Get url for 1 hour download.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="privateBucket">True Means Private Bucket.</param>
        /// <returns>url of file.</returns>
        string GetUrlFor1Hour(string fileName, bool privateBucket = false);
    }
}
