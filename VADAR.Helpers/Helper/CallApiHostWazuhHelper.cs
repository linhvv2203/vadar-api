// <copyright file="CallApiHostWazuhHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Call Api Wazuh Helper.
    /// </summary>
    public class CallApiHostWazuhHelper : ICallApiHostWazuhHelper
    {
        private readonly IConfiguration configs;
        private readonly string wazuhUrl;

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiHostWazuhHelper"/> class.
        /// </summary>
        /// <param name="configs">configs.</param>
        public CallApiHostWazuhHelper(IConfiguration configs)
        {
            this.configs = configs;
            this.wazuhUrl = this.configs["WazuhUrl"];
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiZabbixHelper"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>string.</returns>
        public async Task<string> AddHostWazuh(string name)
        {
            var url = $"{this.wazuhUrl}" + "agents/groups/" + name + "?pretty";
            var request = this.HttpRequestMessage(HttpMethod.Put, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiZabbixHelper"/> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>string.</returns>
        public async Task<string> RemoveHostWazuh(string name)
        {
            var url = $"{this.wazuhUrl}" + "agents/groups/" + name + "?pretty";
            var request = this.HttpRequestMessage(HttpMethod.Delete, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetHostById(string hostIdRef)
        {
            var url = $"{this.wazuhUrl}/agents/{hostIdRef}";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var result = await this.SendRequest(request);

            return result;
        }

        private HttpRequestMessage HttpRequestMessage(HttpMethod method, string uri, string content = "")
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", "Basic " + this.CredentialsEnCoded());
            return request;
        }

        private async Task<string> SendRequest(HttpRequestMessage request)
        {
            using var client = new HttpClient(this.HttpClientHandler());
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        private HttpClientHandler HttpClientHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
            };
        }

        private string CredentialsEnCoded()
        {
            return Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(this.configs["UserWazuh"] + ":" + this.configs["PWWazuh"]));
        }
    }
}
