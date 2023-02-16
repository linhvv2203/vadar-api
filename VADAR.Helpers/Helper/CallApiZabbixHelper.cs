// <copyright file="CallApiZabbixHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Call Api Zabbix Helper.
    /// </summary>
    public class CallApiZabbixHelper : ICallApiZabbixHelper
    {
        private readonly IConfiguration configs;
        private readonly string zabbixUrl;

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiZabbixHelper"/> class.
        /// </summary>
        /// <param name="configs">configs.</param>
        public CallApiZabbixHelper(IConfiguration configs)
        {
            this.configs = configs;
            this.zabbixUrl = this.configs["ZabbixUrl"];
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiZabbixHelper"/> class.
        /// </summary>
        /// <param name="groupids">Group Ids.</param>
        /// <returns>string.</returns>
        public async Task<string> GetHostProblem(string groupids)
        {
            var url = $"{this.zabbixUrl}";
            string result;

            string paramsReq = @"
                    ""output"" : [ ""hostid"", ""host"" ],
                    ""selectInterfaces"" : [  ""interfaceid"", ""ip"" ],
                    ""groupids"":  [ """ + groupids + @""" ] 
                ";
            var request = await this.CreateRequest("host.get", paramsReq);

            var responseData = await this.SendRequest(url, "POST", request);
            result = responseData;

            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetHostByGroup(string groupids)
        {
            var url = $"{this.zabbixUrl}";

            string paramsReq = @"
                    ""output"" : ""extend"",
                   
                     ""groupids"":  [ """ + groupids + @""" ] 
                ";

            var request = await this.CreateRequest("host.get", paramsReq);

            var responseData = await this.SendRequest(url, "POST", request);

            return responseData;
        }

        /// <inheritdoc/>
        public async Task<string> GetAllHost()
        {
            var url = $"{this.zabbixUrl}";

            string paramsReq = @"
                    ""output"" : ""extend""
                ";

            var request = await this.CreateRequest("host.get", paramsReq);

            var responseData = await this.SendRequest(url, "POST", request);

            return responseData;
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
        public async Task<string> AddGroup(string groupName)
        {
            var url = $"{this.zabbixUrl}";
            string paramsReq = @"
                ""name"": """ + groupName + @"""
            ";
            var request = await this.CreateRequest("hostgroup.create", paramsReq);

            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> UpdateGroup(GroupDto groupDto)
        {
            var url = $"{this.zabbixUrl}";
            string paramsReq = @"
                ""groupid"": """ + groupDto.Id + @""",
                ""name"": """ + groupDto.Name + @"""
            ";
            var request = await this.CreateRequest("hostgroup.update", paramsReq);

            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> DeleteGroup(List<int> idGroup)
        {
            var url = $"{this.zabbixUrl}";
            var listRemove = string.Empty;
            foreach (var item in idGroup)
            {
                if (listRemove.Length > 1)
                {
                    listRemove = listRemove + @"""" + "," + @"""" + item;
                }
                else
                {
                    listRemove += item.ToString();
                }
            }

            string paramsReq = @"
                """ + listRemove + @"""
            ";
            var request = await this.CreateRequestDelete("hostgroup.delete", paramsReq);

            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> AddHostToGroup(List<string> hostIds, string groupId)
        {
            var listAdd = string.Empty;
            foreach (var item in hostIds)
            {
                if (listAdd.Length > 1)
                {
                    listAdd = listAdd + "," + "{" + @"""" + "hostid" + @"""" + ":" + @"""" + item + @"""" + "}";
                }
                else
                {
                    listAdd += "{" + @"""" + "hostid" + @"""" + ":" + @"""" + item + @"""" + "}";
                }
            }

            var url = $"{this.zabbixUrl}";

            string paramsReq = @"
                ""groups"" : [{ ""groupid"": " + @"""" + groupId + @"""" + @"}],
                ""hosts"" : [" + listAdd + @"]
            ";
            var request = await this.CreateRequest("hostgroup.massadd", paramsReq);

            var responseData = await this.SendRequest(url, "POST", request);
            return responseData;
        }

        /// <inheritdoc/>
        public async Task<string> RemoveHostFromGroup(List<string> hostIds, List<string> groupId)
        {
            var url = $"{this.zabbixUrl}";
            var listRemove = string.Empty;
            var listGroupId = string.Empty;
            foreach (var item in hostIds)
            {
                if (listRemove.Length > 1)
                {
                    listRemove = listRemove + "," + @"""" + item + @"""";
                }
                else
                {
                    listRemove += @"""" + item + @"""";
                }
            }

            foreach (var item in groupId)
            {
                if (listGroupId.Length > 1)
                {
                    listGroupId = listGroupId + "," + @"""" + item + @"""";
                }
                else
                {
                    listGroupId += @"""" + item + @"""";
                }
            }

            string paramsReq = @"
                ""groupids"" : [" + listGroupId + @"],
                ""hostids"" : [" + listRemove + @"]
            ";
            var request = await this.CreateRequest("hostgroup.massremove", paramsReq);

            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetGroupById(List<int> groupId)
        {
            var listGroupId = string.Empty;
            foreach (var item in groupId)
            {
                if (listGroupId.Length > 1)
                {
                    listGroupId = listGroupId + "," + @"""" + item + @"""";
                }
                else
                {
                    listGroupId += @"""" + item + @"""";
                }
            }

            var url = $"{this.zabbixUrl}";
            var paramsReq = @"
                ""output"": ""extend"",
                ""filter"": {}
            ";
            var request = await this.CreateRequest("hostgroup.get", paramsReq);
            var result = await this.SendRequest(url, "POST", request);

            return result;
        }

        /// <inheritdoc/>
        public async Task<GroupZabbixDto> FindGroupByName(string groupName)
        {
            var url = $"{this.zabbixUrl}";

            string paramsReq = @"
                ""output"": ""extend"",
                ""filter"": { ""name"": [ """ + groupName + @""" ] }
            ";

            var request = await this.CreateRequest("hostgroup.get", paramsReq);
            var dataJson = await this.SendRequest(url, "POST", request);
            var result = this.ConvertJsonDataToGroupDto(dataJson);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> AddWorkspace(string workspaceName)
        {
            var url = $"{this.zabbixUrl}";
            string paramsReq = @"
                ""name"": """ + workspaceName + @"""
            ";
            var request = await this.CreateRequest("hostgroup.create", paramsReq);

            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<WorkspaceZabbixDto> FindWorkspaceByName(string workspaceName)
        {
            var url = $"{this.zabbixUrl}";

            string paramsReq = @"
                ""output"": ""extend"",
                ""filter"": { ""name"": [ """ + workspaceName + @""" ] }
            ";

            var request = await this.CreateRequest("hostgroup.get", paramsReq);
            var dataJson = await this.SendRequest(url, "POST", request);
            var result = this.ConvertJsonDataToWorkspaceDto(dataJson);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetGroups()
        {
            var url = $"{this.zabbixUrl}";
            string paramsReq = @"
                ""output"": ""extend"",
                ""filter"": {}
            ";
            var request = await this.CreateRequest("hostgroup.get", paramsReq);
            var result = await this.SendRequest(url, "POST", request);
            return result;
        }

        /// <inheritdoc/>
        public async Task<string> DeleteHost(List<string> idHost)
        {
            var url = $"{this.zabbixUrl}";
            var listRemove = string.Empty;
            foreach (var item in idHost)
            {
                if (listRemove.Length > 1)
                {
                    listRemove = listRemove + @"""" + "," + @"""" + item;
                }
                else
                {
                    listRemove += item;
                }
            }

            string paramsReq = @"
                """ + listRemove + @"""
            ";
            var request = await this.CreateRequestDelete("host.delete", paramsReq);

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

        private async Task<string> CreateRequestDelete(string method, string paramsReq, int id = 1)
        {
            var auth = method == "user.login" ? "\"auth\": null" : ("\"auth\":" + "\"" + await this.GetTokenZabbix() + "\"");
            return @"{
                ""jsonrpc"": ""2.0"",
                ""method"": """ + method + @""",
                ""params"": [
                   " + paramsReq + @"
                ],
                ""id"": " + id + @",
                " + auth + @"
            }";
        }

        private GroupZabbixDto ConvertJsonDataToGroupDto(string jsonData)
        {
            var result = new GroupZabbixDto();
            if (string.IsNullOrEmpty(jsonData))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            if (data != null && data.result != null)
            {
                foreach (var item in data.result)
                {
                    result.Name = item?.name;
                    result.Id = item?.groupid;

                    return result;
                }
            }

            return result;
        }

        private WorkspaceZabbixDto ConvertJsonDataToWorkspaceDto(string jsonData)
        {
            var result = new WorkspaceZabbixDto();
            if (string.IsNullOrEmpty(jsonData))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            if (data != null && data.result != null)
            {
                foreach (var item in data.result)
                {
                    result.Name = item?.name;
                    result.Id = item?.groupid;

                    return result;
                }
            }

            return result;
        }
    }
}
