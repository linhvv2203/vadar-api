// <copyright file="CallApiWazuhHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Call Api Wazuh Helper.
    /// </summary>
    public class CallApiWazuhHelper : ICallApiWazuhHelper
    {
        private readonly IConfiguration configs;
        private readonly string wazuhUrl;

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiWazuhHelper"/> class.
        /// </summary>
        /// <param name="configs">configs.</param>
        public CallApiWazuhHelper(IConfiguration configs)
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
            var url = $"{this.wazuhUrl}" + "/agents/" + name + "?pretty";
            var request = this.HttpRequestMessage(HttpMethod.Delete, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiZabbixHelper"/> class.
        /// </summary>
        /// <param name="listIds">listIds.</param>
        /// <param name="groupName">groupName.</param>
        /// <returns>string.</returns>
        public async Task<string> AddHostToGroupWazuh(List<string> listIds, string groupName)
        {
            var url = $"{this.wazuhUrl}/agents/groups/{groupName}";

            var listIdAdd = string.Empty;
            foreach (var item in listIds)
            {
                if (listIdAdd.Length > 1)
                {
                    listIdAdd = listIdAdd + "," + @"""" + item + @"""";
                }
                else
                {
                    listIdAdd += @"""" + item + @"""";
                }
            }

            var jsongetDataString = @"{
                ""ids"":[" + listIdAdd + @"]
            }";

            var request = this.HttpRequestMessage(HttpMethod.Put, url, jsongetDataString);
            var responseBody = await this.SendRequest(request);
            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> AddAHostToGroupWazuh(string hostId, string groupName)
        {
            var url = $"{this.wazuhUrl}/agents/{hostId}/group/{groupName}";

            var request = this.HttpRequestMessage(HttpMethod.Put, url);
            var responseBody = await this.SendRequest(request);
            return responseBody;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiZabbixHelper"/> class.
        /// </summary>
        /// <param name="listIds">listIds.</param>
        /// <param name="groupId">groupId.</param>
        /// <returns>string.</returns>
        public async Task<string> RemoveHostFromGroupWazuh(List<string> listIds, string groupId)
        {
            var listIdAdd = string.Empty;
            foreach (var item in listIds)
            {
                if (listIdAdd.Length > 1)
                {
                    listIdAdd = listIdAdd + "," + item;
                }
                else
                {
                    listIdAdd += item;
                }
            }

            var url = $"{this.wazuhUrl}/agents/group/{groupId}?ids={listIdAdd}";
            var request = this.HttpRequestMessage(HttpMethod.Delete, url);

            var responseBody = await this.SendRequest(request);
            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetGroups()
        {
            var url = $"{this.wazuhUrl}/agents/groups?pretty";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetAllHost()
        {
            var url = $"{this.wazuhUrl}/agents";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var result = await this.SendRequest(request);

            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetAllHostByGroup(string groupId)
        {
            var url = $"{this.wazuhUrl}/agents/groups/" + groupId;

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var result = await this.SendRequest(request);

            return result;
        }

        /// <inheritdoc/>
        public async Task<GroupDto> GetGroupDetail(string groupName)
        {
            var url = $"{this.wazuhUrl}/agents/groups?search={groupName}&pretty";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var responseBody = await this.SendRequest(request);
            var groupDto = this.ConvertJsonDataToAGroupDto(responseBody, groupName);

            return groupDto;
        }

        /// <inheritdoc/>
        public async Task<WorkspaceDto> GetWorkspaceDetail(string workspaceName)
        {
            var url = $"{this.wazuhUrl}/agents/groups?search={workspaceName}&pretty";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var responseBody = await this.SendRequest(request);
            var workspaceDto = this.ConvertJsonDataToAWorkspaceDto(responseBody, workspaceName);

            return workspaceDto;
        }

        /// <inheritdoc/>
        public async Task<string> CreateAGroup(string groupName)
        {
            var url = $"{this.wazuhUrl}/agents/groups/{groupName}?pretty";

            var request = this.HttpRequestMessage(HttpMethod.Put, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> CreateAWorkspace(string workspaceName)
        {
            var url = $"{this.wazuhUrl}/agents/groups/{workspaceName}?pretty";

            var request = this.HttpRequestMessage(HttpMethod.Put, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> RemoveAGroup(string groupName)
        {
            var url = $"{this.wazuhUrl}/agents/groups/{groupName}?pretty";

            var request = this.HttpRequestMessage(HttpMethod.Delete, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> RemoveListOfGroups(string[] groupNames)
        {
            var url = $"{this.wazuhUrl}/agents/groups?ids={string.Join(",", groupNames)}";

            var request = this.HttpRequestMessage(HttpMethod.Delete, url);
            var responseBody = await this.SendRequest(request);

            return responseBody;
        }

        private HttpRequestMessage HttpRequestMessage(HttpMethod method, string uri, string content = "")
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };

            // request.Headers.Add("content-type", "application/json");
            request.Headers.Add("Authorization", "Basic " + this.CredentialsEnCoded());

            // request.Headers.Remove("Content-Type");
            // request.Headers.Add("Content-Type", "application/json");
            return request;
        }

        private async Task<string> SendRequest(HttpRequestMessage request)
        {
            using var client = new HttpClient(this.HttpClientHandler());

            // client.DefaultRequestHeaders.Remove("Content-Type");
            // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            // client.DefaultRequestHeaders. .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
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

        private GroupDto ConvertJsonDataToAGroupDto(string jsonData, string groupName)
        {
            var result = new GroupDto();
            if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(groupName))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            if (data != null && data.data?.items != null && data?.data?.items?.Count > 0)
            {
                if (data != null)
                {
                    foreach (var item in data?.data?.items)
                    {
                        if (item?.name.ToString().Trim().ToLower() == groupName.Trim().ToLower())
                        {
                            result.Name = item.name;
                            return result;
                        }
                    }
                }
            }

            return result;
        }

        private WorkspaceDto ConvertJsonDataToAWorkspaceDto(string jsonData, string workspaceName)
        {
            var result = new WorkspaceDto();
            if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(workspaceName))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            if (data != null && data.data?.items != null && data?.data?.items?.Count > 0)
            {
                foreach (var item in data?.data?.items)
                {
                    if (item?.name.ToString().Trim().ToLower() == workspaceName.Trim().ToLower())
                    {
                        result.Name = item.name;
                        return result;
                    }
                }
            }

            return result;
        }
    }
}
