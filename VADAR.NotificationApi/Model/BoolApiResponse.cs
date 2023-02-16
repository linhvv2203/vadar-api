// <copyright file="BoolApiResponse.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json;

namespace VADAR.NotificationApi.Model
{
    /// <summary>
    /// Bool API Response.
    /// </summary>
    public class BoolApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolApiResponse"/> class.
        /// </summary>
        /// <param name="e">Exception.</param>
        public BoolApiResponse(Exception e)
        {
            Success = false;
            ErrorCode = e.HResult;
            Message = e.Message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolApiResponse"/> class.
        /// </summary>
        /// <param name="success">True or false.</param>
        /// <param name="errorCode">error code.</param>
        public BoolApiResponse(bool success, int? errorCode = null)
        {
            Success = success;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolApiResponse"/> class.
        /// </summary>
        /// <param name="success">True or false.</param>
        /// <param name="message">message code.</param>
        /// <param name="errorCode">error code.</param>
        public BoolApiResponse(bool success, string message, int? errorCode)
        {
            Success = success;
            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary>
        /// Gets or sets a value indicating whether.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets message code.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets error code.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ErrorCode { get; set; }
    }
}
