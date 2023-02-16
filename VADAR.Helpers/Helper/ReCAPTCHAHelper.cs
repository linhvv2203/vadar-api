// <copyright file="ReCAPTCHAHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.DTO.ViewModels;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// ReCAPTCHAHelper.
    /// </summary>
    public class RecaptchaHelper : IRecaptchaHelper
    {
        private string urlAPI;
        private string secretKey;

        /// <summary>
        /// Initialises a new instance of the <see cref="RecaptchaHelper"/> class.
        /// Initializes a new instance of the <see cref="RecaptchaHelper"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public RecaptchaHelper(IConfiguration configuration)
        {
            this.urlAPI = configuration["VerifyRe-CaptchaAPI"];
            this.secretKey = configuration["SecretKey-ReCaptcha"];
        }

        /// <inheritdoc/>
        public bool IsValidRecaptcha(string recaptcha)
        {
            if (string.IsNullOrEmpty(recaptcha))
            {
                return false;
            }

            var client = new System.Net.WebClient();

            var googleReply = client.DownloadString($"{this.urlAPI}?secret={this.secretKey}&response={recaptcha}");

            return JsonConvert.DeserializeObject<RecaptchaResponse>(googleReply).Success;
        }
    }
}
