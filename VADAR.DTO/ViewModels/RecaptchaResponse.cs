// <copyright file="RecaptchaResponse.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VADAR.DTO.ViewModels
{
    /// <summary>
    /// ReCAPTCHA Verify Response.
    /// </summary>
    public class RecaptchaResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether verify success or no.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets error codes.
        /// </summary>
        [JsonProperty("error-codes")]
        public IEnumerable<string> ErrorCodes { get; set; }

        /// <summary>
        /// Gets or sets ChallengeTs.
        /// </summary>
        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        /// <summary>
        /// Gets or sets host name.
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}
