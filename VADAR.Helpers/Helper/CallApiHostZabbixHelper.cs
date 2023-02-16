// <copyright file="CallApiHostZabbixHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Call Api Zabbix Helper.
    /// </summary>
    public class CallApiHostZabbixHelper : ICallApiHostZabbixHelper
    {
        private readonly IConfiguration configs;
        private readonly string zabbixUrl;

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiHostZabbixHelper"/> class.
        /// </summary>
        /// <param name="configs">configs.</param>
        public CallApiHostZabbixHelper(IConfiguration configs)
        {
            this.configs = configs;
            this.zabbixUrl = this.configs["ZabbixUrl"];
        }

        /// <summary>
        /// GetTokenZabbix.
        /// </summary>
        /// <returns>string.</returns>
        public async Task<string> GetTokenZabbix()
        {
            var url = $"{this.zabbixUrl}";
            string paramsReq = @"
                ""user"": """ + $"{this.configs["UserZabbix"]}" + @""",
                ""password"": """ + $"{this.configs["PWZabbix"]}" + @"""
            ";
            var request = await this.CreateRequest("user.login", paramsReq, 1);
            var response = await this.SendRequest(url, "POST", request);

            var loginInfo = JsonConvert.DeserializeObject<dynamic>(response);
            string result = loginInfo.result.ToString();
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> AddHost(string hostName, string groupId)
        {
            var url = $"{this.zabbixUrl}";
            string paramsReq = @"
                ""host"": """ + hostName + @""",
               ""groups"" : [{ ""groupid"": " + groupId + @"}],
            ";
            var request = await this.CreateRequest("hostgroup.create", paramsReq);

            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> FindHostByHostId(string hostId)
        {
            var url = $"{this.zabbixUrl}";
            string paramsreq = @"
                 ""output"" : [""hostid""],
                ""selectParentTemplates"" : [""templateid"",""name""],
                  ""hostids"": """ + hostId + @"""
            ";

            var request = await this.CreateRequest("host.get", paramsreq);
            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        private async Task<string> SendRequest(string url, string httpMethod, string jsonData)
        {
            using var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/json-rpc");
            var responseData = await webClient.UploadStringTaskAsync(new Uri(url), httpMethod, jsonData);
            var result = responseData;

            return result;
        }

        private async Task<string> CreateRequest(string method, string paramsReq, int id = 1)
        {
            var auth = method == "user.login" ? "\"auth\": null" : ("\"auth\":" + "\"" + await this.GetTokenZabbix() + "\"");
            return @"{
                ""jsonrpc"": ""2.0"",
                ""method"": """ + method + @""",
                ""params"": {
                   " + paramsReq + @"
                },
                ""id"": " + id + @",
                " + auth + @"
            }";
        }
    }
}
