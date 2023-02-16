// <copyright file="IdentityServerHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// IdentityServerHelper.
    /// </summary>
    public class IdentityServerHelper : IIdentityServerHelper
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityServerHelper"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public IdentityServerHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async System.Threading.Tasks.Task<UserDto> ActiveAccount(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new UserDto();
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri(this.configuration["IdentityServerSetting:IdentityServerUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.configuration["IDAccessKey"]);

            client.Timeout = TimeSpan.FromMinutes(10);
            var response = await client.GetAsync($"/Account/ActiveAccount?email={email}");
            if (response.IsSuccessStatusCode)
            {
                var identityReponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(identityReponse);
                if (result != null && result?.success == true)
                {
                    return new UserDto() { Id = result?.user?.id, UserName = result?.user?.userName, Email = result?.user?.email };
                }
            }

            return new UserDto();
        }

        /// <inheritdoc/>
        public async Task<bool> VerifyEmailExisting(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri(this.configuration["IdentityServerSetting:IdentityServerUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.configuration["SecretKey-ReCaptcha"]);

            client.Timeout = TimeSpan.FromMinutes(10);
            var response = await client.GetAsync($"/Account/VerifyEmailExisting?email={email}");
            if (response.IsSuccessStatusCode)
            {
                var identityReponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(identityReponse);
                if (result != null && result?.success == true)
                {
                    return result?.result;
                }
            }

            return false;
        }
    }
}
