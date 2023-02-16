// <copyright file="TAFHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// 2 authentication factor helper.
    /// </summary>
    public class TAFHelper : ITAFHelper
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initialises a new instance of the <see cref="TAFHelper"/> class.
        /// Blockchain Helper.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public TAFHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Verify 2af code.
        /// </summary>
        /// <param name="verificationCode">verification code.</param>
        /// <param name="token">access token.</param>
        /// <returns>true: success; false: failed.</returns>
        public async Task<bool> VerifyCode(string verificationCode, string token)
        {
            if (string.IsNullOrEmpty(verificationCode) || string.IsNullOrEmpty(token))
            {
                return false;
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri(this.configuration["IdentityServerSetting:IdentityServerUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (token.Length < 9)
            {
                return false;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("/account/OTPCodeVerification?verificationCode=" + verificationCode, null);
            if (response.IsSuccessStatusCode)
            {
                bool.TryParse(response.Content.ReadAsStringAsync().Result, out var result);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Update avatar.
        /// </summary>
        /// <param name="token">access token.</param>
        /// <param name="avatar">avatar.</param>
        /// <returns>true: success; false: failed.</returns>
        public async Task<bool> UpdateAvatar(string token, string avatar)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(avatar))
            {
                return false;
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri(this.configuration["IdentityServerSetting:IdentityServerUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (token.Length < 9)
            {
                return false;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("/account/updateavatar", new StringContent(JsonSerializer.Serialize(new { Url = avatar }), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                bool.TryParse(response.Content.ReadAsStringAsync().Result, out var result);
                return result;
            }

            return false;
        }
    }
}
