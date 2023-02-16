// <copyright file="MiniIOHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Minio;
using VADAR.Exceptions;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Amazone S3 Helper.
    /// </summary>
    public class MiniIoHelper : IMiniIOHelper
    {
        private readonly IStringHelper verifyBase64StringHelper;
        private readonly IConfiguration configuration;
        private readonly string aWss3AccessKeyId;
        private readonly string aWss3AccessKey;
        private readonly string endPoint;

        /// <summary>
        /// Initialises a new instance of the <see cref="MiniIoHelper"/> class.
        /// Initializes a new instance of the <see cref="MiniIoHelper"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="verifyBase64StringHelper">IVerifyBase64StringHelper.</param>
        public MiniIoHelper(IConfiguration configuration, IStringHelper verifyBase64StringHelper)
        {
            this.verifyBase64StringHelper = verifyBase64StringHelper;
            this.configuration = configuration;
            this.aWss3AccessKeyId = configuration["MiniIOAccessKey"];
            this.aWss3AccessKey = configuration["MiniIOSecretKey"];
            this.endPoint = configuration["MiniIOEndPoint"];
        }

        /// <inheritdoc/>
        public async Task<Stream> GetFile(string fileName, bool privateBucket = false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            var bucketName = this.GetBucketName(privateBucket);

            // Issue request and remember to dispose of the response
            Stream returnStream = new MemoryStream();
            await miniIoClient.GetObjectAsync(bucketName, fileName, (stream) =>
            {
                // Uncomment to print the file on output console
                // stream.CopyTo(Console.OpenStandardOutput());
                var responseStream = stream;
                using var ms = new MemoryStream();
                responseStream.CopyTo(ms);

                var bytes = ms.ToArray();
                returnStream = new MemoryStream(bytes);
            });

            return returnStream;
        }

        /// <inheritdoc/>
        public string GetAvatarOfUser(string fileName, bool privateBucket = false)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            fileName = fileName.Split('/').LastOrDefault();

            var bucketName = this.GetBucketName(privateBucket);

            var urlString = (this.endPoint.IndexOf("http", StringComparison.Ordinal) < 0 ? "https://" : string.Empty) + this.endPoint + "/" + bucketName + "/" + fileName;
            return urlString;
        }

        /// <inheritdoc/>
        public string GetUrlFor13Hour(string fileName, bool privateBucket = false)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            var bucketName = this.GetBucketName(privateBucket);

            fileName = fileName.Split('/').LastOrDefault();

            // var reqParams = new Dictionary<string, string> { { "response-content-type", "application/json" }, { "response-content-disposition", "attachment; filename=\"" + fileName + "\"" } };
            // string urlString = miniIOClient.PresignedGetObjectAsync(bucketName, fileName, 46800, reqParams).Result;
            var urlString = miniIoClient.PresignedGetObjectAsync(bucketName, fileName, 46800).Result;

            return urlString;
        }

        /// <inheritdoc/>
        public string GetUrlFor1Hour(string fileName, bool privateBucket = false)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            var bucketName = this.GetBucketName(privateBucket);

            fileName = fileName.Split('/').LastOrDefault();

            // var reqParams = new Dictionary<string, string> { { "response-content-type", "application/json" }, { "response-content-disposition", "attachment; filename=\"" + fileName + "\"" } };
            // string urlString = miniIOClient.PresignedGetObjectAsync(bucketName, fileName, 3600, reqParams).Result;
            var urlString = miniIoClient.PresignedGetObjectAsync(bucketName, fileName, 3600).Result;

            return urlString;
        }

        /// <inheritdoc/>
        public async Task<string> UploadFile(string base64Content, string fileName, string contentType, bool privateBucket = false)
        {
            if (!this.verifyBase64StringHelper.IsBase64(base64Content))
            {
                return null;
            }

            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            var bucketName = this.GetBucketName(privateBucket);

            if (!await miniIoClient.BucketExistsAsync(bucketName))
            {
                await miniIoClient.MakeBucketAsync(bucketName);
            }

            var timeStamp = this.CreateTimeStamp();

            fileName = timeStamp + "." + fileName.Split(".").LastOrDefault();

            var contentBytes = Convert.FromBase64String(base64Content);

            var fileStream = new MemoryStream(contentBytes);
            await miniIoClient.PutObjectAsync(bucketName, fileName, fileStream, fileStream.Length, !string.IsNullOrEmpty(contentType) ? contentType : "application/octet-stream");

            return bucketName + "/" + fileName;
        }

        /// <inheritdoc/>
        public async Task<string> UploadFile(byte[] contentBytes, string fileName, string contentType, bool privateBucket = false, bool isCensorshipOrganizationFile = false)
        {
            if (contentBytes == null || string.IsNullOrEmpty(fileName))
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            var bucketName = this.GetBucketName(privateBucket);

            if (!await miniIoClient.BucketExistsAsync(bucketName))
            {
                await miniIoClient.MakeBucketAsync(bucketName);
            }

            if (!isCensorshipOrganizationFile)
            {
                var timeStamp = this.CreateTimeStamp();

                fileName = timeStamp + "." + fileName.Split(".").LastOrDefault();
            }

            var fileStream = new MemoryStream(contentBytes);

            await miniIoClient.PutObjectAsync(bucketName, fileName, fileStream, fileStream.Length, !string.IsNullOrEmpty(contentType) ? contentType : "application/octet-stream");

            return bucketName + "/" + fileName;
        }

        /// <inheritdoc/>
        public async Task DeleteFile(string fileUrl, bool privateBucket = false)
        {
            var bucketName = this.GetBucketName(privateBucket);

            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            if (await miniIoClient.BucketExistsAsync(bucketName))
            {
                await miniIoClient.RemoveObjectAsync(bucketName, fileUrl.Replace((this.endPoint.IndexOf("http", StringComparison.Ordinal) < 0 ? "https://" : string.Empty) + this.endPoint + "/" + bucketName + "/", string.Empty));
            }
        }

        /// <inheritdoc/>
        public async Task<long> GetSizeFile(string fileName, bool privateBucket = false)
        {
            var miniIoClient = new MinioClient(this.endPoint, this.aWss3AccessKeyId, this.aWss3AccessKey).WithSSL();

            try
            {
                var meta = await miniIoClient.StatObjectAsync(this.GetBucketName(privateBucket), fileName);
                return meta.Size;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private string CreateTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        }

        private string GetBucketName(bool isPrivate)
        {
            return isPrivate ? this.configuration["MiniIOBucketPrivate"] : this.configuration["MiniIOBucket"];
        }
    }
}
