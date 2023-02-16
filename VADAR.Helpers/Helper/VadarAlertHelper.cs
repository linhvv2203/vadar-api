// <copyright file="VadarAlertHelper.cs" company="VSEC">
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
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Vadar Alert Helper.
    /// </summary>
    public class VadarAlertHelper : IVadarAlertHelper
    {
        private readonly IConfiguration configs;
        private readonly string vadarApiUrl;
        private readonly string vadarFileUrl;

        /// <summary>
        /// Initialises a new instance of the <see cref="VadarAlertHelper"/> class.
        /// </summary>
        /// <param name="configs">configs.</param>
        public VadarAlertHelper(IConfiguration configs)
        {
            this.configs = configs;
            this.vadarApiUrl = this.configs["VadarCoreApi"];
            this.vadarFileUrl = this.configs["VadarFileUrl"];
        }

        /// <inheritdoc/>
        public async Task<List<string>> ListEmailAlerts(string wokrkspaceName)
        {
            var jobId = await this.GetIdAlerts(wokrkspaceName);
            if (jobId > 0)
            {
                await Task.Delay(5000);
            }

            var alertRequest = this.HttpRequestMessage(HttpMethod.Get, $"{this.vadarApiUrl}/api/v2/jobs/{jobId}/job_events/");
            var alertResponseJson = await this.SendRequest(alertRequest);
            var listAlert = this.GetListEmailAlert(alertResponseJson);

            return listAlert;
        }

        /// <inheritdoc/>
        public async Task<bool> InitAlerts(AlertsDto alertsDto)
        {
            var url = $"{this.vadarApiUrl}/api/v2/job_templates/{this.configs["Alert:Init"]}/launch/";

            var dataRequest = @"{
                ""extra_vars"": {
                    ""email"": """ + $"{alertsDto.Email}" + @""",
                    ""workspace_name"": """ + $"{alertsDto.WorkspaceName}" + @"""
                }
            }";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, dataRequest);
            var responseJson = await this.SendRequest(request);
            var result = this.VerifyResult(responseJson);
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> AddEmailToAlerts(AlertsDto alertsDto)
        {
            var url = $"{this.vadarApiUrl}/api/v2/job_templates/{this.configs["Alert:AddEmail"]}/launch/";

            var dataRequest = @"{
                ""extra_vars"": {
                    ""email"": """ + $"{alertsDto.Email}" + @""",
                    ""workspace_name"": """ + $"{alertsDto.WorkspaceName}" + @"""
                }
            }";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, dataRequest);
            var responseJson = await this.SendRequest(request);
            var result = this.VerifyResult(responseJson);
            if (result)
            {
                await Task.Delay(5000);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveEmailFromAlerts(AlertsDto alertsDto)
        {
            var url = $"{this.vadarApiUrl}/api/v2/job_templates/{this.configs["Alert:RemoveEmail"]}/launch/";

            var dataRequest = @"{
                ""extra_vars"": {
                    ""email"": """ + $"{alertsDto.Email}" + @""",
                    ""workspace_name"": """ + $"{alertsDto.WorkspaceName}" + @"""
                }
            }";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, dataRequest);
            var responseJson = await this.SendRequest(request);
            var result = this.VerifyResult(responseJson);
            if (result)
            {
                await Task.Delay(9000);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<AgentInstallDto>> BuildAgentCentosForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto)
        {
            var url = $"{this.vadarApiUrl}/api/v2/job_templates/17/launch/";
            var dist = buildAgentForWorkspaceDto.IsProd ? "prod" : "dev";

            var dataRequest = @"{
                ""extra_vars"": {
                    ""TOKEN"": """ + $"{buildAgentForWorkspaceDto.Token}" + @""",
                    ""NAME"": """ + $"{buildAgentForWorkspaceDto.Name}" + @""",
                    ""DIST"": """ + $"{dist}" + @""",
                    ""OS"": """ + $"{buildAgentForWorkspaceDto.Os}" + @"""
                }
            }";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, dataRequest);
            var responseJson = await this.SendRequest(request);
            var result = new List<AgentInstallDto>();
            if (string.IsNullOrEmpty(responseJson))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(responseJson);
            if (data?.id > 0)
            {
                var version = "1.0";
                var prefixSecurity = "vadars-agent";
                var prefixPerformance = "vadarp-agent";
                var urlLink = this.vadarFileUrl + buildAgentForWorkspaceDto.Token + "/" + buildAgentForWorkspaceDto.Os + "/" + version + "/";

                result.Add(
                    new AgentInstallDto
                    {
                        LinkDownload = urlLink + prefixSecurity + "-3.11.4-1.x86_64.rpm",
                        Name = EnAgentInstallType.Security.ToString(),
                        Version = version,
                        Status = (int)EnAgentInstallStatus.Active,
                    });

                result.Add(
                    new AgentInstallDto
                    {
                        LinkDownload = urlLink + prefixPerformance + "-4.4.9-1.el7.x86_64.rpm",
                        Name = EnAgentInstallType.Performance.ToString() + "(Centos7)",
                        Version = version,
                        Status = (int)EnAgentInstallStatus.Active,
                    });
            }

            return result;
        }

        /// <inheritdoc/>
        public List<AgentInstallDto> BuildAgentCentosForWorkspaceV2(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto)
        {
            var result = new List<AgentInstallDto>();
            if (string.IsNullOrEmpty(buildAgentForWorkspaceDto.Token))
            {
                return result;
            }

            var version = "1.0";
            var urlLink = this.GetUrlLink(buildAgentForWorkspaceDto);
            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadarp-agent-4.4.9-1.el7.x86_64.rpm",
                    Name = EnAgentInstallType.Performance.ToString(),
                    Status = (int)EnAgentInstallStatus.Active,
                    Version = version,
                });

            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadars-agent-3.11.4-1.x86_64.rpm",
                    Name = EnAgentInstallType.Security.ToString(),
                    Version = version,
                    Status = (int)EnAgentInstallStatus.Active,
                });

            return result;
        }

        /// <inheritdoc/>
        public List<AgentInstallDto> BuildAgentMacForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto)
        {
            var result = new List<AgentInstallDto>();
            if (string.IsNullOrEmpty(buildAgentForWorkspaceDto.Token))
            {
                return result;
            }

            var version = "1.0";
            var urlLink = this.GetUrlLink(buildAgentForWorkspaceDto);
            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadarp_macOs_agent.pkg",
                    Name = EnAgentInstallType.Performance.ToString(),
                    Status = (int)EnAgentInstallStatus.Active,
                    Version = version,
                });

            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadars_macOs_agent.pkg",
                    Name = EnAgentInstallType.Security.ToString(),
                    Version = version,
                    Status = (int)EnAgentInstallStatus.Active,
                });

            return result;
        }

        /// <inheritdoc/>
        public List<AgentInstallDto> BuildAgentWindowForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto)
        {
            var result = new List<AgentInstallDto>();
            if (string.IsNullOrEmpty(buildAgentForWorkspaceDto.Token))
            {
                return result;
            }

            var version = "1.0";
            var urlLink = this.GetUrlLink(buildAgentForWorkspaceDto);
            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadar_performance.exe",
                    Name = EnAgentInstallType.Performance.ToString(),
                    Status = (int)EnAgentInstallStatus.Active,
                    Version = version,
                });

            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadar_security.msi",
                    Name = EnAgentInstallType.Security.ToString(),
                    Version = version,
                    Status = (int)EnAgentInstallStatus.Active,
                });

            return result;
        }

        /// <inheritdoc/>
        public List<AgentInstallDto> BuildAgentUbuntuForWorkspace(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto)
        {
            var result = new List<AgentInstallDto>();
            if (string.IsNullOrEmpty(buildAgentForWorkspaceDto.Token))
            {
                return result;
            }

            var version = "1.0";
            var urlLink = this.GetUrlLink(buildAgentForWorkspaceDto);
            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadarp-agent_4.4.8-1+bionic_amd64.deb",
                    Name = EnAgentInstallType.Performance.ToString(),
                    Status = (int)EnAgentInstallStatus.Active,
                    Version = version,
                });

            // result.Add(
            //    new AgentInstallDto
            //    {
            //        LinkDownload = urlLink + "vadarp-agent_4.4.9-1+focal_amd64.deb",
            //        Name = EnAgentInstallType.Performance.ToString(),
            //        Status = (int)EnAgentInstallStatus.Active,
            //        Version = version,
            //    });
            result.Add(
                new AgentInstallDto
                {
                    LinkDownload = urlLink + "vadars-agent_3.11.3-1_amd64.deb",
                    Name = EnAgentInstallType.Security.ToString(),
                    Version = version,
                    Status = (int)EnAgentInstallStatus.Active,
                });

            return result;
        }

        private string GetUrlLink(BuildAgentForWorkspaceDto buildAgentForWorkspaceDto)
        {
            var urlLink = $"https://{this.configs["MiniIOEndPoint"]}/{this.configs["MiniIOBucket"]}/{buildAgentForWorkspaceDto.Folders}";
            return urlLink;
        }

        private HttpRequestMessage HttpRequestMessage(HttpMethod method, string uri, string content = "")
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", "Basic YWRtaW46czBubTFuaEBT"); // + this.CredentialsEnCoded());
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

        private List<string> GetListEmailAlert(string jsonData)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(jsonData))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            if (data == null || data?.results == null)
            {
                return result;
            }

            foreach (var item in data?.results)
            {
                if (item?.event_display.ToString().ToLower() == "host ok" && item?.task.ToString().ToLower() == "list email in alerts")
                {
                    foreach (var e in item?.event_data?.res?.stdout_lines)
                    {
                        if (!string.IsNullOrEmpty(e.ToString()))
                        {
                            result.Add(e.ToString());
                        }
                    }
                }
            }

            return result;
        }

        private async Task<int> GetIdAlerts(string workspaceName)
        {
            var dataRequest = @"{
                ""extra_vars"": {
                    ""workspace_name"": """ + $"{workspaceName}" + @"""
                }
            }";

            var jobRequest = this.HttpRequestMessage(HttpMethod.Post, $"{this.vadarApiUrl}/api/v2/job_templates/{this.configs["Alert:ListEmail"]}/launch/", dataRequest);
            var jobResponseJson = await this.SendRequest(jobRequest);
            var result = 0;
            if (string.IsNullOrEmpty(jobResponseJson))
            {
                return result;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jobResponseJson);
            if (data?.id >= 0)
            {
                result = data?.id;
                return result;
            }

            return result;
        }

        private bool VerifyResult(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return false;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            if (data?.id >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
