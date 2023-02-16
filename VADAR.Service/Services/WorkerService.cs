// <copyright file="WorkerService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;
using static VADAR.Helpers.Const.Constants;

namespace VADAR.Service.Services
{
    /// <summary>
    /// Worker service.
    /// </summary>
    public class WorkerService : IWorkerService
    {
        private readonly string sourcePath = Directory.GetCurrentDirectory();
        private readonly IConfiguration configuration;
        private readonly IMiniIOHelper miniIoHelper;
        private readonly string queueName;
        private readonly string connectionString;
        private readonly IWorkspaceUnitOfWork workspaceUnitOfWork;
        private readonly IDashboardUnitOfWork dashboardUnitOfWork;
        private readonly IWorkspaceHostUnitOfWork workspaceHostUnitOfWork;
        private readonly IRedisCachingHelper redisCachingHelper;
        private readonly IElasticSearchCallApiHelper elasticSearchCallApiHelper;
        private IQueueClient queueClient;
        private readonly IStringHelper stringHelper;

        /// <summary>
        /// Initialises a new instance of the <see cref="WorkerService"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        /// <param name="miniIoHelper">miniIOHelper.</param>
        /// <param name="workspaceUnitOfWork">workspaceUnitOfWork.</param>
        /// <param name="dashboardUnitOfWork">Dashboard Unit Of Work.</param>
        /// <param name="redisCachingHelper">Redis Caching Helper.</param>
        /// <param name="elasticSearchCallApiHelper">Elasticsearch Call Api Helper.</param>
        /// <param name="workspaceHostUnitOfWork">Workspace Host Unit Of Work.</param>
        /// <param name="stringHelper">stringHelper.</param>
        public WorkerService(
            IConfiguration configuration,
            IMiniIOHelper miniIoHelper,
            IWorkspaceUnitOfWork workspaceUnitOfWork,
            IDashboardUnitOfWork dashboardUnitOfWork,
            IRedisCachingHelper redisCachingHelper,
            IElasticSearchCallApiHelper elasticSearchCallApiHelper,
            IWorkspaceHostUnitOfWork workspaceHostUnitOfWork,
            IStringHelper stringHelper)
        {
            this.configuration = configuration;
            this.queueName = this.configuration["ServiceBus:QueueName"];
            this.connectionString = this.configuration["ServiceBus:ServiceBusConnectionString"];
            this.queueClient = new QueueClient(
              this.connectionString, this.queueName);
            this.miniIoHelper = miniIoHelper;
            this.workspaceUnitOfWork = workspaceUnitOfWork;
            this.dashboardUnitOfWork = dashboardUnitOfWork;
            this.redisCachingHelper = redisCachingHelper;
            this.elasticSearchCallApiHelper = elasticSearchCallApiHelper;
            this.workspaceHostUnitOfWork = workspaceHostUnitOfWork;
            this.stringHelper = stringHelper;
        }

        /// <inheritdoc/>
        public async Task WorkerChangeStatusLicense()
        {
            var listWorkspace = (await this.workspaceUnitOfWork.WorkspaceRepository.GetAll())
                .Include(license => license.License)
                .Where(x => x.License.Status != (int)EnLicenseStatus.Revoked)
                .ToList();

            var now = DateTime.Now;

            foreach (var item in listWorkspace)
            {
                var license = item.License;
                var licenseEndTime = item.License?.EndDate;
                if (licenseEndTime == null)
                {
                    continue;
                }

                var dateLicense = (DateTime)licenseEndTime;
                Console.WriteLine($"*** {now.Subtract(now)}");
                var checkTime = dateLicense.Subtract(now);

                var aInterval = new TimeSpan(0, 168, 0, 0);
                var timeSendMail = dateLicense.Add(aInterval);
                var checkTimeSendMail = timeSendMail.Subtract(now);

                if (checkTime.Ticks <= 0 && checkTimeSendMail.Ticks > 0)
                {
                    license.Status = (int)EnLicenseStatus.ExpiredDate;
                    item.License = license;
                    Console.WriteLine($"*** Update status expride");
                }
                else
                {
                    if (checkTimeSendMail.Ticks < 0)
                    {
                        license.Status = (int)EnLicenseStatus.Revoked;
                        item.License = license;
                        Console.WriteLine($"*** Update status revoked");
                    }
                    else
                    {
                        Console.WriteLine($"*** continue license");
                    }
                }

                if (checkTimeSendMail.Ticks >= 0 && checkTime.Ticks <= 0)
                {
                    license.Status = (int)EnLicenseStatus.ExpiredDate;
                    item.License = license;

                    var workspaceNotifications = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository.GetAll())
                .Where(x => x.WorkspaceId == item.Id && !string.IsNullOrWhiteSpace(x.Address));

                    foreach (var itemEmail in workspaceNotifications)
                    {
                        // send mail
                        using var client = new HttpClient { BaseAddress = new Uri(this.configuration["UrlApiNoti"]) };
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        Console.WriteLine("***** START SEND EMAIL ****");

                        var notiRequest = new SendNotificationRequest
                        {
                            ReceiverId = itemEmail.Address,
                            Message = "Tai khoan dung thu cua ban sap het han",
                            Type = "VAD_LICENSE",
                        };
                        var json = JsonConvert.SerializeObject(notiRequest);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                        await client.PostAsync("/api/Notification", data);
                    }

                    Console.WriteLine($"*** send mail");
                }

                await this.workspaceUnitOfWork.WorkspaceRepository.Edit(item);
            }

            _ = this.workspaceUnitOfWork.Commit();
        }

        /// <inheritdoc/>
        public async Task CheckEmailReport()
        {
            try
            {
                var listWorkspace = (await this.workspaceUnitOfWork.WorkspaceRepository.GetAll())
                .Include(license => license.License)
                .Where(x => x.License.Status != (int)EnLicenseStatus.Revoked)
                .ToList();
                Console.WriteLine($"listWorkspace: {JsonConvert.SerializeObject(listWorkspace.Select(x => x.Name))}");

                foreach (var item in listWorkspace)
                {
                    var workspaceNotifications = (await this.workspaceUnitOfWork.WorkspaceNotificationRepository.GetAll())
                    .Where(x => x.WorkspaceId == item.Id && !string.IsNullOrWhiteSpace(x.Address));
                    Console.WriteLine($"workspace: {item.Name} ---- workspaceNotifications: {JsonConvert.SerializeObject(workspaceNotifications.Select(x => x.Address))}");

                    var redisKey = item.Id.ToString();
                    var valueRedis = await this.redisCachingHelper.GetDataByKey(redisKey);
                    Console.WriteLine($"valueRedis: {valueRedis}");
                    if (valueRedis != null)
                    {
                        continue;
                    }

                    var hostWorkspace = await this.GetHostsOfWorkSpace(null, item.Id);

                    _ = this.redisCachingHelper.SetObjectData(redisKey, item.Name, 10);
                    var dataRequest = new LogSecurityRequestDto
                    {
                        RequestUserId = null,
                        WorkspaceId = item.Id,
                    };
                    dataRequest.Hosts = hostWorkspace;
                    dataRequest.FromDate = DateTime.UtcNow.AddDays(-1);
                    dataRequest.ToDate = DateTime.UtcNow;

                    var dataRequestPerformance = new LogsPerformanceRequestDto
                    {
                        RequestUserId = null,
                        WorkspaceId = item.Id,
                        Hosts = hostWorkspace,
                        FromDate = DateTime.UtcNow.AddDays(-1),
                        ToDate = DateTime.UtcNow,
                    };

                    Console.WriteLine($"dataRequest.Hosts.Count: {dataRequest.Hosts.Count}");
                    if (dataRequest.Hosts.Count <= 0)
                    {
                        continue;
                    }

                    var responseString = await this.elasticSearchCallApiHelper.GetLogSecurity(dataRequest);
                    var resultSend = this.GetLogSecurity(responseString);

                    var responseStringPerformance = await this.elasticSearchCallApiHelper.GetPerformanceLog(dataRequestPerformance);
                    var resultSendPerformance = await this.GetLogsPerformanceAsync(responseStringPerformance, (int)dataRequestPerformance.WorkspaceId);

                    Console.WriteLine($"resultSend.Count: {resultSend.Count}");
                    Console.WriteLine($"resultSendPerformance.Count: {resultSendPerformance.Count}");
                    dynamic myDynamic = new
                    {
                        CountSecurity = resultSend.Count,
                        CountPerformance = resultSendPerformance.Count,
                    };

                    // send mail
                    Console.WriteLine($"send mail HttpClient ");
                    using var client = new HttpClient { BaseAddress = new Uri(this.configuration["UrlApiNoti"]) };
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    foreach (var address in workspaceNotifications)
                    {
                        Console.WriteLine("***** START SEND EMAIL ****");

                        var notiRequest = new SendNotificationRequest
                        {
                            ReceiverId = address.Address,
                            Message = myDynamic,
                            Type = "VAD_REPORT",
                            AccessKey = this.configuration["AccessKey"],
                        };
                        var json = JsonConvert.SerializeObject(notiRequest);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("/api/Notification", data);
                        var reponseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"responseString: {reponseString}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task ReceiveMessages()
        {
            this.queueClient = new QueueClient(this.connectionString, this.queueName);

            // Register the queue message handler and receive messages in a loop
            this.RegisterOnMessageHandlerAndReceiveMessages();
            Console.ReadKey();

            await this.queueClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(this.ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false,
            };

            // Register the function that processes messages.
            this.queueClient.RegisterMessageHandler(this.ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            var messageContent = Encoding.UTF8.GetString(message.Body);

            var buildAgentForWorkspace = JsonConvert.DeserializeObject<BuildAgentForWorkspaceDto>(messageContent);
            if (buildAgentForWorkspace != null && !string.IsNullOrEmpty(buildAgentForWorkspace.Token))
            {
                switch (buildAgentForWorkspace.Os)
                {
                    case Os.Window:
                        // build wazuh.
                        this.MoveFileAdjustToWazuhSourceFolder(buildAgentForWorkspace);
                        this.RunFileBuildAgent(this.configuration["Windows:Wazuh:SourceWazuhPath"], this.configuration["Windows:Wazuh:WazuhInstallerBuildMsi"]);

                        // push wazuh file install to server.
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        var agentWazuhInstallationPath = $"{this.configuration["Windows:Wazuh:SourceWazuhPath"]}{this.configuration["Windows:Wazuh:WazuhFileNameInstall"]}";
                        var contentBytes = this.GetBytesFromPath(agentWazuhInstallationPath);
                        var fileInstallationPath = buildAgentForWorkspace.Folders + this.configuration["Windows:Wazuh:WazuhFileNameInstall"];
                        await this.miniIoHelper.UploadFile(contentBytes, fileInstallationPath, this.configuration["Windows:Wazuh:WazuhFileNameInstall"].Split(".").LastOrDefault(), isCensorshipOrganizationFile: true);

                        // build zabbix.
                        this.MoveFileAdjustToZabbixSourceFolder(buildAgentForWorkspace);
                        this.RunFileBuildAgent(this.configuration["Windows:Zabbix:SourceZabbixPath"], $"makensis {this.configuration["Windows:Zabbix:VadarUpdaterFile"]}");

                        // push zabbix file install to server.
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        var agentZabbixInstallationPath = $"{this.configuration["Windows:Zabbix:SourceZabbixPath"]}{this.configuration["Windows:Zabbix:ZabbixFileNameInstall"]}";
                        var contentZabbixBytes = this.GetBytesFromPath(agentZabbixInstallationPath);
                        var fileZabbixInstallationPath = buildAgentForWorkspace.Folders + this.configuration["Windows:Zabbix:ZabbixFileNameInstall"];
                        await this.miniIoHelper.UploadFile(contentZabbixBytes, fileZabbixInstallationPath, this.configuration["Windows:Zabbix:ZabbixFileNameInstall"].Split(".").LastOrDefault(), isCensorshipOrganizationFile: true);

                        await this.queueClient.CompleteAsync(message.SystemProperties.LockToken);
                        break;
                    case Os.Ubuntu:

                        // wazuh security.
                        var buildSecAgent = $"TOKEN={buildAgentForWorkspace.Token} NAME={buildAgentForWorkspace.Name} ACTION=build_sec_agent sh {this.configuration["Ubuntu:Wazuh:BuildShFile"]}";
                        Console.WriteLine($"{nameof(buildSecAgent)}: {buildSecAgent}");
                        var cmd = this.RunCommand(buildSecAgent);
                        Console.WriteLine($"{nameof(cmd)}: {cmd}");
                        await this.UploadFileForUbuntu(buildAgentForWorkspace, this.configuration["Ubuntu:Wazuh:FileNameInstall"]);

                        // zabbix performance 18.
                        var buildPerfAgentU18 = $"TOKEN={buildAgentForWorkspace.Token} NAME={buildAgentForWorkspace.Name} ACTION=build_perf_agent_u18 sh {this.configuration["Ubuntu:Wazuh:BuildShFile"]}";
                        Console.WriteLine($"{nameof(buildPerfAgentU18)}: {buildPerfAgentU18}");
                        var cmd2 = this.RunCommand(buildPerfAgentU18);
                        Console.WriteLine($"{nameof(cmd2)}: {cmd2}");
                        await this.UploadFileForUbuntu(buildAgentForWorkspace, this.configuration["Ubuntu:Zabbix:FileNameInstall_18"]);

                        // zabbix performance 20.
                        var buildPerfAgentU20 = $"TOKEN={buildAgentForWorkspace.Token} NAME={buildAgentForWorkspace.Name} ACTION=build_perf_agent_u20 sh {this.configuration["Ubuntu:Wazuh:BuildShFile"]}";
                        Console.WriteLine($"{nameof(buildPerfAgentU20)}: {buildPerfAgentU20}");
                        var cmd3 = this.RunCommand(buildPerfAgentU20);
                        Console.WriteLine($"{nameof(cmd3)}: {cmd3}");
                        await this.UploadFileForUbuntu(buildAgentForWorkspace, this.configuration["Ubuntu:Zabbix:FileNameInstall_20"]);

                        await this.queueClient.CompleteAsync(message.SystemProperties.LockToken);
                        break;

                    case Os.Macos:

                        // wazuh security.
                        var macBuildSecAgent = $"TOKEN={buildAgentForWorkspace.Token} NAME={buildAgentForWorkspace.Name} ACTION=build_sec_agent_mac sh /root/sourcecode/build.sh";
                        Console.WriteLine($"{nameof(macBuildSecAgent)}: {macBuildSecAgent}");
                        var cmdWazuhMac = this.RunCommand(macBuildSecAgent);
                        Console.WriteLine($"{nameof(cmdWazuhMac)}: {cmdWazuhMac}");
                        await this.UploadFileForMac(buildAgentForWorkspace, this.configuration["MacOs:Wazuh:FileNameInstall"]);

                        // zabbix performance.
                        var macBuildPerfAgent = $"TOKEN={buildAgentForWorkspace.Token} NAME={buildAgentForWorkspace.Name} ACTION=build_perf_agent_mac sh /root/sourcecode/build.sh";
                        Console.WriteLine($"{nameof(macBuildPerfAgent)}: {macBuildPerfAgent}");
                        var cmdZabbixMac = this.RunCommand(macBuildPerfAgent);
                        Console.WriteLine($"{nameof(cmdZabbixMac)}: {cmdZabbixMac}");
                        await this.UploadFileForMac(buildAgentForWorkspace, this.configuration["MacOs:Zabbix:FileNameInstall"]);

                        await this.queueClient.CompleteAsync(message.SystemProperties.LockToken);
                        break;
                }
            }
        }

        private async Task UploadFileForMac(BuildAgentForWorkspaceDto buildAgentForWorkspace, string fileName)
        {
            if (buildAgentForWorkspace == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMinutes(3));
            var ubuntuSourceWazuhPath = $"{this.configuration["Mac:Wazuh:SourceWazuhPath"]}{buildAgentForWorkspace.Name}/{fileName}";
            Console.WriteLine($"SourceWazuhPath: {ubuntuSourceWazuhPath}");
            var ubuntuContentWazuhBytes = this.GetBytesFromPath(ubuntuSourceWazuhPath);
            var ubuntuFileWazuhInstallation = $"agents/{Os.Macos.ToLower()}/{buildAgentForWorkspace.Token}/" + fileName;
            Console.WriteLine($"ubuntuFileWazuhInstallation: agents/{Os.Macos.ToLower()}/{buildAgentForWorkspace.Name}/" + fileName);
            var rs = await this.miniIoHelper.UploadFile(ubuntuContentWazuhBytes, ubuntuFileWazuhInstallation, fileName.Split(".").LastOrDefault(), isCensorshipOrganizationFile: true);
            Console.WriteLine($"res: {rs}");
        }

        private async Task UploadFileForUbuntu(BuildAgentForWorkspaceDto buildAgentForWorkspace, string fileName)
        {
            if (buildAgentForWorkspace == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMinutes(3));
            var ubuntuSourceWazuhPath = $"{this.configuration["Ubuntu:Wazuh:SourceWazuhPath"]}{buildAgentForWorkspace.Name}/{fileName}";
            Console.WriteLine($"SourceWazuhPath: {ubuntuSourceWazuhPath}");
            var ubuntuContentWazuhBytes = this.GetBytesFromPath(ubuntuSourceWazuhPath);
            var ubuntuFileWazuhInstallation = $"agents/{Os.Ubuntu.ToLower()}/{buildAgentForWorkspace.Token}/" + fileName;
            Console.WriteLine($"ubuntuFileWazuhInstallation: agents/{Os.Ubuntu.ToLower()}/{buildAgentForWorkspace.Name}/" + fileName);
            var rs = await this.miniIoHelper.UploadFile(ubuntuContentWazuhBytes, ubuntuFileWazuhInstallation, fileName.Split(".").LastOrDefault(), isCensorshipOrganizationFile: true);
            Console.WriteLine($"res: {rs}");
        }

        private bool RunCommand(string command)
        {
            var procStartInfo = new ProcessStartInfo();
            procStartInfo.FileName = "/bin/bash";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.Arguments = $"-c \"{command}\"";
            Console.WriteLine($"cmd: -c \"{command}\"");

            var proc = new Process();
            proc.StartInfo = procStartInfo;
            return proc.Start();
        }

        private void MoveFileAdjustToWazuhSourceFolder(BuildAgentForWorkspaceDto buildAgentForWorkspace)
        {
            // file Invoke.
            this.CreateFile(this.configuration["Windows:Wazuh:InvokeFileName"], this.GetInvokeFileConfig(buildAgentForWorkspace));
            this.CopyFileToFolder(this.sourcePath, this.configuration["Windows:Wazuh:SourceWazuhPath"], this.configuration["Windows:Wazuh:InvokeFileName"]);

            // file Wazuh Install File.
            this.CreateFile(this.configuration["Windows:Wazuh:WazuhInstallFile"], this.GetWazuhIntallerFileConfig(buildAgentForWorkspace));
            this.CopyFileToFolder(this.sourcePath, this.configuration["Windows:Wazuh:SourceWazuhPath"], this.configuration["Windows:Wazuh:WazuhInstallFile"]);

            // file Wazuh Installer Build Msi.
            this.CreateFile(this.configuration["Windows:Wazuh:WazuhInstallerBuildMsi"], this.GetWazuhInStallerBuild());
            this.CopyFileToFolder(this.sourcePath, this.configuration["Windows:Wazuh:SourceWazuhPath"], this.configuration["Windows:Wazuh:WazuhInstallerBuildMsi"]);
        }

        private void MoveFileAdjustToZabbixSourceFolder(BuildAgentForWorkspaceDto buildAgentForWorkspace)
        {
            var auth = this.configuration["Windows:Zabbix:Auth"];
            var zabbixConfig = this.GetZabbixFileConfig(buildAgentForWorkspace, auth);
            this.CreateFile(this.configuration["Windows:Zabbix:VadarUpdaterFile"], zabbixConfig);
            this.CopyFileToFolder(this.sourcePath, this.configuration["Windows:Zabbix:SourceZabbixPath"], this.configuration["Windows:Zabbix:VadarUpdaterFile"]);

            var vadarAgentd = this.GetVadarAgentd();
            this.CreateFile(this.configuration["Windows:Zabbix:VadarAgentdFile"], vadarAgentd);
            this.CopyFileToFolder(this.sourcePath, this.configuration["Windows:Zabbix:SourceZabbixConfPath"], this.configuration["Windows:Zabbix:VadarAgentdFile"]);

            var vadarPsk = this.GetVadarPsk();
            this.CreateFile(this.configuration["Windows:Zabbix:Vadar_psk_File"], vadarPsk);
            this.CopyFileToFolder(this.sourcePath, this.configuration["Windows:Zabbix:SourceZabbixConfPath"], this.configuration["Windows:Zabbix:Vadar_psk_File"]);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private void CopyFileToFolder(string srcPath, string targetPath, string fileName)
        {
            var sourceWazuhInstallFile = Path.Combine(srcPath, fileName);
            var destWazuhInstallFile = Path.Combine(targetPath, fileName);

            File.Copy(sourceWazuhInstallFile, destWazuhInstallFile, true);
        }

        private void RunFileBuildAgent(string dir, string fileName)
        {
            Process proc;
            try
            {
                proc = new Process();
                proc.StartInfo.WorkingDirectory = dir;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.Arguments = $"/c {fileName}";
                proc.Start();
                proc.WaitForExit();
                Console.WriteLine("Bat file executed !!");
            }
            catch (Exception ex)
            {
                if (ex.StackTrace != null)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void CreateFile(string fileName, string value)
        {
            using var sw = File.CreateText(fileName);
            sw.WriteLine(value);
        }

        private byte[] GetBytesFromPath(string fileName)
        {
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // Create a byte array of file stream length.
            var bytes = File.ReadAllBytes(fileName);

            // Read block of bytes from stream into the byte array.
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

            // Close the File Stream.
            fs.Close();

            return bytes; // return the byte data.
        }

        private string GetInvokeFileConfig(BuildAgentForWorkspaceDto buildAgentForWorkspace)
        {
            var invokeFileConfig = @"Start-Sleep -Seconds 10
$FilePath = ""${env:UserProfile}\powershell.msi.test.log""            

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
[string]$os = [environment]::OSVersion.Version

$keyPath = $scriptPath + ""\client.keys""

$key  = Get-Content -Path $keyPath

$wazuh_ref = $key.Split("" "")[0]

#$wazuh_ref | Out-File -Encoding ASCII -FilePath $

$machine_id = (Get-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\vadar -Name ""MachineID"").MachineID
if (!$machine_id){
    $machine_id = (Get-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\vadar -Name ""MachineID"").MachineID
    
    if(!$machine_id){
        $machine_id = '{'+[guid]::NewGuid().ToString()+'}'
		New-Item -Path HKLM:\SOFTWARE\WOW6432Node\vadar -Value ""default value""
        New-ItemProperty -Path ""HKLM:\SOFTWARE\WOW6432Node\vadar"" -Name ""MachineID"" -Value $machine_id  -PropertyType ""String""
    }
}

[string]$Agent_name = $env:computername

$url = ""https://fb1ffb16f77afe091b4502f1783aadc3.m.pipedream.net""
$url_vadar = """ + this.configuration["Windows:Wazuh:ApiHost"] + @"""

  
[string]$data =""{
  `""name`"": `""" + buildAgentForWorkspace.Name.ToLower() + @"-$Agent_name`"",
  `""description`"": `""`"",
  `""os`"": `""Microsoft Windows $os`"",
  `""status`"": 1,
  `""tokenWorkspace`"": `""" + buildAgentForWorkspace.Token + @"`"",
  `""MACHINE_ID`"": `""$machine_id`"",
  `""wazuhRef`"": `""$wazuh_ref`""
  }""
  
Out-File -FilePath .\Process.txt -InputObject $data -Encoding ASCII

$curl_path = $scriptPath + ""\curl.exe""
$output_path = $scriptPath + ""\output.txt""

& $curl_path --request POST $url_vadar --header ""Content-Type: application/json"" -d ""@Process.txt"" | Out-File -FilePath $output_path
& $curl_path --request POST $url_vadar --header ""Content-Type: application/json"" -d ""@Process.txt"" | Out-File -FilePath $output_path
& $curl_path --request POST $url_vadar --header ""Content-Type: application/json"" -d ""@Process.txt"" | Out-File -FilePath $output_path
& $curl_path --request POST $url_vadar --header ""Content-Type: application/json"" -d ""@Process.txt"" | Out-File -FilePath $output_path

#Start-Service OssecSvc
#Write-Output $Agent_name";

            return invokeFileConfig;
        }

        private string GetWazuhIntallerFileConfig(BuildAgentForWorkspaceDto buildAgentForWorkspace)
        {
            var wazuhIntallerConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Wix xmlns=""http://schemas.microsoft.com/wix/2006/wi"" xmlns:util=""http://schemas.microsoft.com/wix/UtilExtension"">
    <Product Id=""*"" Name=""Vadar Security Agent"" Language=""1033"" Version=""3.11.4"" Manufacturer=""VSEC, JSC."" UpgradeCode=""F495AC57-7BDE-4C4B-92D8-DBE40A9AA5A0"">
        <Package Description=""Vadar helps you to gain security visibility into your infrastructure by monitoring hosts at an operating system and application level. It provides the following capabilities: log analysis, file integrity monitoring, intrusions detection and policy and compliance monitoring"" Comments=""vadar-agent"" InstallerVersion=""200"" Compressed=""yes"" InstallPrivileges=""elevated""/>
        <Media Id=""1"" Cabinet=""simple.cab"" EmbedCab=""yes"" CompressionLevel=""high"" />
        <!-- Default configuration values -->
        <Property Id=""WAZUH_MANAGER"" Secure=""yes"" Value=""" + this.configuration["Windows:Wazuh:WAZUH_MANAGER"] + @""">
        </Property>
        <Property Id=""WAZUH_MANAGER_PORT"" Secure=""yes"" Value=""" + this.configuration["Windows:Wazuh:WAZUH_MANAGER_PORT"] + @""">
        </Property>
        <Property Id=""WAZUH_PROTOCOL"" Secure=""yes"" Value=""TCP"">
        </Property>
        <Property Id=""WAZUH_REGISTRATION_SERVER"" Secure=""yes"" Value=""" + this.configuration["Windows:Wazuh:WAZUH_REGISTRATION_SERVER"] + @""">
        </Property>
        <Property Id=""WAZUH_REGISTRATION_PORT"" Secure=""yes"" Value=""" + this.configuration["Windows:Wazuh:WAZUH_REGISTRATION_PORT"] + @"""></Property>
        <Property Id=""WAZUH_REGISTRATION_PASSWORD"" Secure=""yes"" Value=""" + this.configuration["Windows:Wazuh:WAZUH_REGISTRATION_PASSWORD"] + @""">
        </Property>
        <Property Id=""WAZUH_KEEP_ALIVE_INTERVAL"" Secure=""yes"">
        </Property>
        <Property Id=""WAZUH_TIME_RECONNECT"" Secure=""yes"">
        </Property>
        <Property Id=""WAZUH_REGISTRATION_CA"" Secure=""yes"">
        </Property>
        <Property Id=""WAZUH_REGISTRATION_CERTIFICATE"" Secure=""yes"">
        </Property>
        <Property Id=""WAZUH_REGISTRARTION_KEY"" Secure=""yes"">
        </Property>
        <Property Id=""WAZUH_AGENT_NAME"" Secure=""yes"">
        </Property>
        <Property Id=""WAZUH_AGENT_GROUP"" Secure=""yes"" Value=""" + buildAgentForWorkspace.Name + @""">
        </Property>
        <!-- Deprecated options -->
        <Property Id=""ADDRESS"" Secure=""yes"">
        </Property>
        <Property Id=""GROUP"" Secure=""yes"">
        </Property>
        <Property Id=""SERVER_PORT"" Secure=""yes"">
        </Property>
        <Property Id=""NOTIFY_TIME"" Secure=""yes"">
        </Property>
        <Property Id=""TIME_RECONNECT"" Secure=""yes"">
        </Property>
        <Property Id=""AUTHD_SERVER"" Secure=""yes"">
        </Property>
        <Property Id=""AUTHD_PORT"" Secure=""yes"">1515</Property>
        <Property Id=""PROTOCOL"" Secure=""yes"">
        </Property>
        <Property Id=""PASSWORD"" Secure=""yes"">
        </Property>
        <Property Id=""CERTIFICATE"" Secure=""yes"">
        </Property>
        <Property Id=""PEM"" Secure=""yes"">
        </Property>
        <Property Id=""KEY"" Secure=""yes"">
        </Property>
        <Property Id=""AGENT_NAME"" Secure=""yes"">
        </Property>
        <Property Id=""MsiLogging"" Value=""v"" />
        <Icon Id=""icon.ico"" SourceFile=""ui\favicon.ico"" />
        <Property Id=""ARPPRODUCTICON"" Value=""icon.ico"" />
        <Property Id=""WixAppFolder"" Value=""WixPerMachineFolder"" />
        <WixVariable Id=""WixUISupportPerUser"" Value=""0"" />
        <Property Id=""ALLUSERS"" Value=""1"" />
        <WixVariable Id=""WixUILicenseRtf"" Value=""license.rtf"" />
        <WixVariable Id=""WixUIBannerBmp"" Value=""ui\bannrbmp.jpg"" />
        <WixVariable Id=""WixUIDialogBmp"" Value=""ui\dlgbmp.jpg"" />
        <Property Id=""WIXUI_EXITDIALOGOPTIONALCHECKBOXTEXT"" Value=""Run Agent configuration interface"" />
        <Property Id=""WixShellExecTarget"" Value=""[#WIN32UI.EXE]"" />
        <Property Id=""ARPNOMODIFY"" Value=""yes"" />
        <Property Id=""ARPNOREPAIR"" Value=""yes"" />
        <Property Id=""ApplicationFolderName"" Value=""ossec-agent"" />
		
		
		
		<Property Id=""POWERSHELLEXE"">
		<RegistrySearch Id=""POWERSHELLEXE""
                  Type=""raw""
                  Root=""HKLM""
                  Key=""SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell""
                  Name=""Path"" />
		</Property>
		
		<Condition Message=""This application requires Windows PowerShell."">
		  <![CDATA[Installed OR POWERSHELLEXE]]>
		</Condition>
		
		<SetProperty Id=""InvokeTestPS1""
             Before=""InvokeTestPS1""
             Sequence=""execute""
             Value =""&quot;[POWERSHELLEXE]&quot; -NoProfile -NonInteractive -InputFormat None -ExecutionPolicy Bypass -Command &quot;&amp; '[#InvokeTestPS1]' ; exit $$($Error.Count)&quot;"" />

		<CustomAction Id=""InvokeTestPS1""
					  BinaryKey=""WixCA""
					  DllEntry=""CAQuietExec""
					  Execute=""deferred""
					  Impersonate=""no"" />


        <CustomAction Id=""LaunchApplication"" BinaryKey=""WixCA"" DllEntry=""WixShellExec"" Impersonate=""no"" />

        <Binary Id=""InstallerScripts"" SourceFile=""InstallerScripts.vbs"" />

        <!-- This script will remove all of the files and folders from the root folder on uninstall, *except* some files indicated in RemoveAllScript.vbs. -->
        <!-- Especially, ""ossec.conf"" and ""client.keys"" will be kept, and a couple of other files too. -->
        <Binary Id=""RemoveAllScript"" SourceFile=""RemoveAllScript.vbs"" />

        <CustomAction Id=""SetRemoveAllDataValue"" Return=""check"" Property=""CustomAction_RemoveAllScript"" Value=""&quot;[APPLICATIONFOLDER]&quot;"" />
        <CustomAction Id=""CustomAction_RemoveAllScript"" BinaryKey=""RemoveAllScript"" VBScriptCall=""removeAll"" Return=""check"" Execute=""commit"" Impersonate=""no"" />

        <CustomAction Id=""SetCustomActionDataValue"" Return=""check"" Property=""CustomAction_InstallerScripts"" Value=""&quot;[APPLICATIONFOLDER]&quot;,&quot;[ADDRESS]&quot;,&quot;[SERVER_PORT]&quot;,&quot;[PROTOCOL]&quot;,&quot;[NOTIFY_TIME]&quot;,&quot;[TIME_RECONNECT]&quot;,&quot;[WAZUH_MANAGER]&quot;,&quot;[WAZUH_MANAGER_PORT]&quot;,&quot;[WAZUH_PROTOCOL]&quot;,&quot;[WAZUH_KEEP_ALIVE_INTERVAL]&quot;,&quot;[WAZUH_TIME_RECONNECT]&quot;"" />
        <CustomAction Id=""CustomAction_InstallerScripts"" BinaryKey=""InstallerScripts"" VBScriptCall=""config"" Return=""check"" Execute=""commit"" Impersonate=""no"" />

        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT]"" />
        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />

        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD]"" />
        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />

        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT]"" />
        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />

        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD]"" />
        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />
        <CustomAction Id=""CustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot;"" />

        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />

        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute='deferred' Impersonate='no' Return=""check"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -G [WAZUH_AGENT_GROUP] -A " + buildAgentForWorkspace.Name.ToLower() + @"-[ComputerName]"" />
        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_NoName_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />

        <CustomAction Id=""CustomAction_Group_Name_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />

        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />
        <CustomAction Id=""CustomAction_Group_Name_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [WAZUH_REGISTRATION_SERVER] -A [WAZUH_AGENT_NAME] -p [WAZUH_REGISTRATION_PORT] -P [WAZUH_REGISTRATION_PASSWORD] -k &quot;[WAZUH_REGISTRATION_KEY]&quot; -x &quot;[WAZUH_REGISTRATION_CERTIFICATE]&quot; -v &quot;[WAZUH_REGISTRATION_CA]&quot; -G [WAZUH_AGENT_GROUP]"" />

        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT]"" />
        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -v &quot;[CERTIFICATE]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot;"" />

        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD]"" />
        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -v &quot;[CERTIFICATE]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot;"" />

        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT]"" />
        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -v &quot;[CERTIFICATE]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot;"" />

        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD]"" />
        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -v &quot;[CERTIFICATE]&quot;"" />
        <CustomAction Id=""DepCustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot;"" />

        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />

        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -G [GROUP] -A [COMPUTERNAME]"" />
        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_NoName_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />

        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />

        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_Password"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_Password_VerifyAgent"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_Password_VerifyManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />
        <CustomAction Id=""DepCustomAction_Group_Name_RunAuthd_Password_VerifyAgentManager"" Directory=""APPLICATIONFOLDER"" Execute=""immediate"" ExeCommand=""[APPLICATIONFOLDER]agent-auth.exe -m [AUTHD_SERVER] -A [AGENT_NAME] -p [AUTHD_PORT] -P [PASSWORD] -k &quot;[KEY]&quot; -x &quot;[PEM]&quot; -v &quot;[CERTIFICATE]&quot; -G [GROUP]"" />

        <CustomAction Id=""StartWinService"" Directory=""APPLICATIONFOLDER"" ExeCommand=""NET START &quot;OssecSvc&quot;"" Execute=""deferred"" Return=""ignore"" />
        <CustomAction Id=""DepStartWinService"" Directory=""APPLICATIONFOLDER"" ExeCommand=""NET START &quot;OssecSvc&quot;"" Execute=""immediate"" Return=""ignore"" />

        <!-- Explicitely stopping the service is needed in order to allow the deletion of the files by RemoveAllScript.vbs. -->
        <!-- Otherwise, the locked files and directories will not be deleted. -->
        <CustomAction Id=""StopWinService"" Directory=""APPLICATIONFOLDER"" ExeCommand=""NET STOP &quot;OssecSvc&quot;"" Execute=""deferred"" Return=""ignore"" />
        <UI>
            <UIRef Id=""WixUI_Advanced"" />
            <Publish Dialog=""ExitDialog"" Control=""Finish"" Event=""DoAction"" Value=""LaunchApplication"">WIXUI_EXITDIALOGOPTIONALCHECKBOX = 1 and NOT Installed</Publish>
        </UI>
        <InstallExecuteSequence>
            <Custom Action=""SetCustomActionDataValue"" After=""InstallInitialize"">NOT Installed</Custom>
            <Custom Action=""CustomAction_InstallerScripts"" After=""SetCustomActionDataValue"">NOT Installed</Custom>

            <!-- Stop the service as soon as possible on uninstall, and only on uninstall, in order to unlock the files and folders that must be deleted. -->
            <Custom Action=""StopWinService"" After=""InstallInitialize"">(NOT UPGRADINGPRODUCTCODE) AND (REMOVE=""ALL"")</Custom>
            <Custom Action=""SetRemoveAllDataValue"" Before=""InstallFinalize"">(NOT UPGRADINGPRODUCTCODE) AND (REMOVE=""ALL"")</Custom>
            <Custom Action=""CustomAction_RemoveAllScript"" After=""SetRemoveAllDataValue"">(NOT UPGRADINGPRODUCTCODE) AND (REMOVE=""ALL"")</Custom>
			
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP = """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_Group_NoName_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_NoName_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_Group_NoName_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_NoName_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_Group_NoName_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_NoName_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_Group_NoName_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_NoName_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME = """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_Group_Name_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_Name_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_Group_Name_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_Name_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD = """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""CustomAction_Group_Name_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_Name_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA = """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>
            <Custom Action=""CustomAction_Group_Name_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE = """" AND WAZUH_REGISTRATION_KEY = """"</Custom>
            <Custom Action=""CustomAction_Group_Name_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND WAZUH_REGISTRATION_SERVER &lt;&gt; """" AND WAZUH_AGENT_GROUP &lt;&gt; """" AND WAZUH_AGENT_NAME &lt;&gt; """" AND WAZUH_REGISTRATION_PASSWORD &lt;&gt; """" AND WAZUH_REGISTRATION_CA &lt;&gt; """" AND WAZUH_REGISTRATION_CERTIFICATE &lt;&gt; """" AND WAZUH_REGISTRATION_KEY &lt;&gt; """"</Custom>

            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_NoName_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_NoGroup_Name_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP = """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_NoName_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME = """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD = """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_Password"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_Password_VerifyAgent"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE = """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_Password_VerifyManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM = """" AND KEY = """"</Custom>
            <Custom Action=""DepCustomAction_Group_Name_RunAuthd_Password_VerifyAgentManager"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """" AND GROUP &lt;&gt; """" AND AGENT_NAME &lt;&gt; """" AND PASSWORD &lt;&gt; """" AND CERTIFICATE &lt;&gt; """" AND PEM &lt;&gt; """" AND KEY &lt;&gt; """"</Custom>
            <Custom Action=""DepStartWinService"" Before=""InstallFinalize"">NOT Installed AND AUTHD_SERVER &lt;&gt; """"</Custom>
			<Custom Action=""InvokeTestPS1"" Before=""InstallFinalize"">
				<![CDATA[NOT Installed]]>
			</Custom>
        </InstallExecuteSequence>
        <Directory Id=""TARGETDIR"" Name=""SourceDir"">
            <Directory Id=""ProgramFilesFolder"" Name=""PFiles"">
                <Directory Id=""APPLICATIONFOLDER"" Name=""ossec-agent"">
                    <Component Id=""AGENT_AUTH.EXE"" DiskId=""1"" Guid=""F99FEE7C-A021-4D43-9119-98A8D72EAB65"">
                        <File Id=""AGENT_AUTH.EXE"" Name=""agent-auth.exe"" Source=""agent-auth.exe"" >
                         </File>
                        <File Id=""AGENT_AUTH.EXE.MANIFEST"" Name=""agent-auth.exe.manifest"" Source=""agent-auth.exe.manifest"" />
						<File Id=""InvokeTestPS1"" Name=""Invoke-Test.ps1"" Source=""Invoke-Test.ps1"" />
						<File Id=""CURL.EXE"" Name=""curl.exe"" Source=""curl.exe"" />
						<File Id=""CURL_CA_BUNDLE.CRT"" Name=""curl-ca-bundle.crt"" Source=""curl-ca-bundle.crt"" />
                    </Component>
				
                    <Component Id=""SYSCOLLECTOR_DLL"" DiskId=""1"" Guid=""3F72347F-1282-44C1-8A97-40BB68CAE4FB"">
                        <File Id=""SYSCOLLECTOR_DLL"" Name=""syscollector_win_ext.dll"" Source=""..\wazuh_modules\syscollector\syscollector_win_ext.dll"" />
                    </Component>
                    <Component Id=""LIBWAZUHEXT_DLL"" DiskId=""1"" Guid=""31F7B5FA-9678-427B-9536-88AAA897A82A"">
                        <File Id=""LIBWAZUHEXT_DLL"" Name=""libwazuhext.dll"" Source=""..\libwazuhext.dll"" />
                    </Component>

                    <!-- Mark this file as permanent, so it is not deleted on uninstall. -->
                    <Component Id=""LOCAL_INTERNAL_OPTIONS.CONF"" DiskId=""1"" Guid=""10245598-2EE7-4EDB-A114-5398F01A21F9"" NeverOverwrite=""yes"" Permanent=""yes"">
                        <File Id=""LOCAL_INTERNAL_OPTIONS.CONF"" Name=""local_internal_options.conf"" Source=""default-local_internal_options.conf"" KeyPath=""yes"" />
                    </Component>

                    <Component Id=""OSSEC_PRE6.CONF"" DiskId=""1"" Guid=""71947F0E-56D6-4E14-8539-214DFF6CA2BE"" NeverOverwrite=""yes"" Permanent=""yes"">
                        <File Id=""OSSEC_PRE6.CONF"" Name=""ossec.conf"" Source=""default-ossec-pre6.conf"" KeyPath=""yes"">
                            <util:PermissionEx User=""Everyone"" GenericRead=""yes"" GenericWrite=""yes"" />
                        </File>
                        <Condition>VersionNT &lt; 600</Condition>
                    </Component>
                    <Component Id=""OSSEC.CONF"" DiskId=""1"" Guid=""26C3265E-EFC8-488D-8D19-397A0C44C071"" NeverOverwrite=""yes"" Permanent=""yes"">
                        <File Id=""OSSEC.CONF"" Name=""ossec.conf"" Source=""default-ossec.conf"" KeyPath=""yes"">
                            <util:PermissionEx User=""Everyone"" GenericRead=""yes"" GenericWrite=""yes"" />
                        </File>
                        <Condition>VersionNT &gt;= 600</Condition>
                    </Component>
                    <Component Id=""INTERNAL_OPTIONS.CONF"" DiskId=""1"" Guid=""D2F2A5B9-1A98-4BB8-8AC4-D948CA97DD0E"">
                        <File Id=""INTERNAL_OPTIONS.CONF"" Name=""internal_options.conf"" Source=""internal_options.conf"" DefaultVersion=""1.0""/>
                    </Component>
                    <Component Id=""LICENSE.TXT"" DiskId=""1"" Guid=""556F08A0-D372-4BB5-BC44-73CE45957084"">
                        <File Id=""LICENSE.TXT"" Name=""LICENSE.txt"" Source=""LICENSE.txt"" />
                    </Component>
                    <Component Id=""LIBWINPTHREAD_1.DLL"" DiskId=""1"" Guid=""C15C5883-00FB-41D7-B9E6-53C8BC30761F"">
                        <File Id=""LIBWINPTHREAD_1.DLL"" Name=""libwinpthread-1.dll"" Source=""libwinpthread-1.dll"" />
                    </Component>
                    <Component Id=""MANAGE_AGENTS.EXE"" DiskId=""1"" Guid=""C15C5883-00FB-41D7-B7E6-53C8BC30761F"">
                        <File Id=""MANAGE_AGENTS.EXE"" Name=""manage_agents.exe"" Source=""manage_agents.exe"" />
                    </Component>
                    <Component Id=""OSSEC_AGENT_EVENTCHANNEL.EXE"" DiskId=""1"" Guid=""044E7997-12B6-4178-BD00-B90500DBA53F"">
                        <File Id=""OSSEC_AGENT_EVENTCHANNEL.EXE"" Name=""ossec-agent.exe"" Source=""ossec-agent-eventchannel.exe"" />
                        <ServiceInstall Name=""OssecSvc"" Type=""ownProcess"" Start=""auto"" ErrorControl=""normal"" Description=""Wazuh Windows Agent"" DisplayName=""Wazuh"" Id=""svc_install_eventchannel"" />
                        <ServiceControl Id=""svc_uninstall_eventchannel"" Name=""OssecSvc"" Remove=""uninstall"" Stop=""uninstall"" Wait=""yes"" />
                        <Condition>VersionNT &gt;= 600</Condition>
                    </Component>
                    <Component Id=""OSSEC_AGENT.EXE"" DiskId=""1"" Guid=""5CCEA6DC-8434-4137-9486-55AE3949266B"">
                        <File Id=""OSSEC_AGENT.EXE"" Name=""ossec-agent.exe"" Source=""ossec-agent.exe"" />
                        <ServiceInstall Name=""OssecSvc"" Type=""ownProcess"" Start=""auto"" ErrorControl=""normal"" Description=""Wazuh Windows Agent"" DisplayName=""Wazuh"" Id=""svc_install"" />
                        <ServiceControl Id=""svc_uninstall"" Name=""OssecSvc"" Remove=""uninstall"" Stop=""uninstall"" Wait=""yes"" />
                        <Condition>VersionNT &lt; 600</Condition>
                    </Component>
                    <Component Id=""VISTA_SEC.TXT"" DiskId=""1"" Guid=""20EF5801-369B-4EC2-87A2-59DCE56308D9"">
                        <File Id=""VISTA_SEC.TXT"" Name=""vista_sec.txt"" Source=""vista_sec.txt"" />
                    </Component>
                    <Component Id=""WIN32UI.EXE"" DiskId=""1"" Guid=""E7ACBC6F-D8A0-410B-B8D2-2AD9F5152BA0"">
                        <File Id=""WIN32UI.EXE"" Name=""win32ui.exe"" Source=""os_win32ui.exe"">
                        </File>
                        <File Id=""WIN32UI.EXE.MANIFEST"" Name=""win32ui.exe.manifest"" Source=""win32ui.exe.manifest"" />
                    </Component>
                    <Component Id=""REMOVE_OLD_NSIS"" Guid=""3536239B-022D-4A9B-A7F8-2F64132115ED"">
                        <RemoveRegistryKey Action=""removeOnInstall"" Key=""SOFTWARE\ossec"" Root=""HKLM"" />
                        <RemoveRegistryKey Action=""removeOnInstall"" Key=""Software\Microsoft\Windows\CurrentVersion\Uninstall\OSSEC"" Root=""HKLM"" />
                        <RemoveFile Id=""NSIS_UNINSTALL_EXE"" Name=""uninstall.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_OSSEC_LUA_EXE"" Name=""ossec-lua.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_OSSEC_LUAC_EXE"" Name=""ossec-luac.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_ROOTCHECK_EXE"" Name=""ossec-rootcheck.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_SETUP_IIS_EXE"" Name=""setup-iis.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_SYSCHECK_EXE"" Name=""setup-syscheck.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_SETUP_WINDOWS_EXE"" Name=""setup-windows.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_FAVICON_ICO"" Name=""favicon.ico"" On=""install"" />
                        <RemoveFile Id=""NSIS_DOC_HTML"" Name=""doc.html"" On=""install"" />
                        <RemoveFile Id=""NSIS_ADD_LOCALFILE_EXE"" Name=""add-localfile.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_WIN32UI_EXE"" Name=""win32ui.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_AGENT_AUTH_EXE"" Name=""agent-auth.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_LIBWINPTHREAD_1_DLL"" Name=""libwinpthread-1.dll"" On=""install"" />
                        <RemoveFile Id=""NSIS_MANAGE_AGENTS_EXE"" Name=""manage_agents.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_OSSEC_AGENT_EXE"" Name=""ossec-agent.exe"" On=""install"" />
                        <RemoveFile Id=""NSIS_VERSION_TXT"" Name=""VERSION.txt"" On=""install"" />
                        <RemoveFile Id=""NSIS_HELP_TXT"" Name=""help.txt"" On=""install"" />
                    </Component>
                    <Component DiskId=""1"" Guid=""21A074CB-3BFB-45D2-A0EC-D59293950DD9"" Id=""HELP_WIN.TXT"">
                        <File Id=""HELP_WIN.TXT"" Name=""help.txt"" Source=""help_win.txt"" />
                    </Component>
                    <Component Id=""VERSION"" DiskId=""1"" Guid=""8DC3D417-5663-4E53-9D8F-2CFA08A2627C"">
                        <File Id=""VERSION"" Name=""VERSION"" Source=""VERSION"" />
                    </Component>
                    <Component Id=""REVISION"" DiskId=""1"" Guid=""89440258-B50F-4926-8068-D1444E31F8E0"">
                        <File Id=""REVISION"" Name=""REVISION"" Source=""REVISION"" />
                    </Component>
                    <Component Id=""WPK_ROOT.PEM"" DiskId=""1"" Guid=""EABF8773-57B9-4CD8-A862-87B0E060DBF8"">
                        <File Id=""WPK_ROOT.PEM"" Name=""wpk_root.pem"" Source=""..\..\etc\wpk_root.pem"" />
                    </Component>
                    <Directory Id=""ACTIVE_RESPONSE"" Name=""active-response"">
                        <Directory Id=""BIN"" Name=""bin"">
                            <Component Id=""RESTART_OSSEC.CMD"" DiskId=""1"" Guid=""5A405DD9-F4FF-4313-B242-A28DE03611CA"">
                                <File Id=""RESTART_OSSEC.CMD"" Name=""restart-ossec.cmd"" Source=""restart-ossec.cmd"" />
                            </Component>
                            <Component Id=""ROUTE_NULL.CMD"" DiskId=""1"" Guid=""249F3287-B69D-46F0-9EB8-3FED24998E07"">
                                <File Id=""ROUTE_NULL.CMD"" Name=""route-null.cmd"" Source=""route-null.cmd"" />
                            </Component>
                            <Component Id=""ROUTE_NULL_2012.CMD"" DiskId=""1"" Guid=""C59AD066-EB30-4C67-B476-4CFA8D74947D"">
                                <File Id=""ROUTE_NULL_2012.CMD"" Name=""route-null-2012.cmd"" Source=""route-null-2012.cmd"" />
                            </Component>
                            <Component Id=""NETSH.CMD"" DiskId=""1"" Guid=""292E9082-56BE-4258-9224-7F36A59CA433"">
                                <File Id=""NETSH.CMD"" Name=""netsh.cmd"" Source=""netsh.cmd"" />
                            </Component>
                            <Component Id=""NETSH_WIN_2016.CMD"" DiskId=""1"" Guid=""B2011FD2-3F82-4FFD-B6C3-32665037C4EA"">
                                <File Id=""NETSH_WIN_2016.CMD"" Name=""netsh-win-2016.cmd"" Source=""netsh-win-2016.cmd"" />
                            </Component>
                        </Directory>
                        <Component Id=""ACTIVE_RESPONSES.LOG"" DiskId=""1"" Guid=""249F3287-B69D-46F0-8888-3FED24998E07"">
                            <File Id=""ACTIVE_RESPONSES.LOG"" Name=""active-responses.log"" Source=""active-responses.log"" />
                        </Component>
                    </Directory>
                    <Directory Id=""SHARED"" Name=""shared"">
                        <Component Id=""ROOTKIT_FILES.TXT"" DiskId=""1"" Guid=""FE45C8B7-CD37-4E13-B6CA-5838771DF2C2"">
                            <File Id=""ROOTKIT_FILES.TXT"" Name=""rootkit_files.txt"" Source=""..\rootcheck\db\rootkit_files.txt"" />
                        </Component>
                        <Component Id=""ROOTKIT_TROJANS.TXT"" DiskId=""1"" Guid=""6A2D5202-A610-4E00-B6E3-41FA24EA8B88"">
                            <File Id=""ROOTKIT_TROJANS.TXT"" Name=""rootkit_trojans.txt"" Source=""..\rootcheck\db\rootkit_trojans.txt"" />
                        </Component>
                        <Component Id=""WIN_APPLICATIONS_RCL.TXT"" DiskId=""1"" Guid=""833B42BC-7BEF-4801-A91D-737774F05800"">
                            <File Id=""WIN_APPLICATIONS_RCL.TXT"" Name=""win_applications_rcl.txt"" Source=""..\rootcheck\db\win_applications_rcl.txt"" />
                        </Component>
                        <Component Id=""WIN_AUDIT_RCL.TXT"" DiskId=""1"" Guid=""DB5DA081-B508-43CF-B83B-97649697636D"">
                            <File Id=""WIN_AUDIT_RCL.TXT"" Name=""win_audit_rcl.txt"" Source=""..\rootcheck\db\win_audit_rcl.txt"" />
                        </Component>
                        <Component Id=""WIN_MALWARE_RCL.TXT"" DiskId=""1"" Guid=""8FFA7C93-43A4-4946-B3B6-2255D8BFEA11"">
                            <File Id=""WIN_MALWARE_RCL.TXT"" Name=""win_malware_rcl.txt"" Source=""..\rootcheck\db\win_malware_rcl.txt"" />
                        </Component>
                    </Directory>
                    <Directory Id=""RULESET"" Name=""ruleset"">
                      <Directory Id=""SECURITY_CONFIGURATION_ASSESSMENT"" Name=""sca"">
                        <Component Id=""REMOVE_OLD_POLICIES"" Guid=""1007B392-F0CF-401E-B670-6EFAD5A374BA"">
                          <RemoveFile Id=""remove_policies"" Name=""*"" On=""install""/>
                        </Component>
                        <Component Id=""SCA_WIN_AUDIT.YML"" DiskId=""1"" Guid=""1164B8AA-1968-48D3-BAEB-68E6E0BFDBD8"">
                          <File Id=""SCA_WIN_AUDIT.YML"" Name=""sca_win_audit.yml"" Source=""..\..\etc\sca\windows\sca_win_audit.yml"" />
                        </Component>
                      </Directory>
                    </Directory>
                    <Directory Id=""TMP"" Name=""tmp"" />
                    <Directory Id=""QUEUE"" Name=""queue"">
                        <Directory Id=""DIFF"" Name=""diff"" />
                    </Directory>
                    <Directory Id=""BOOKMARKS"" Name=""bookmarks"" />
                    <Directory Id=""LOGS"" Name=""logs"" />
                    <Directory Id=""WODLES"" Name=""wodles"" />
                    <Directory Id=""RIDS"" Name=""rids"" />
                    <Directory Id=""SYSCHECK"" Name=""syscheck"" />
                    <Directory Id=""INCOMING"" Name=""incoming"" />
                    <Directory Id=""UPGRADE"" Name=""upgrade"" />
                </Directory>
            </Directory>
            <Directory Id=""ProgramMenuFolder"">
                <Directory Id=""ProgramMenuDir"" Name=""OSSEC"">
                    <Component Id=""StartMenuShortcuts"" Guid=""6C151D64-A90E-48A0-853C-FDEE0BD628C5"">
                        <RemoveFolder Id=""ProgramMenuDir"" On=""uninstall"" />
                        <RegistryValue Root=""HKCU"" Key=""Software\[Manufacturer]\[ProductName]"" Type=""string"" Value=""[Version]"" />
                        <Shortcut Id=""EDIT_CONF"" Name=""Edit conf"" Target=""[APPLICATIONFOLDER]ossec.conf"" WorkingDirectory=""APPLICATIONFOLDER"" />
                        <Shortcut Id=""UninstallProduct"" Name=""Uninstall"" Description=""Uninstalls the application"" Target=""[System64Folder]msiexec.exe"" Arguments=""/x [ProductCode]"" />
                        <Shortcut Id=""RUN_WIN32UI"" Name=""Manage Agent"" Target=""[APPLICATIONFOLDER]win32ui.exe"" WorkingDirectory=""APPLICATIONFOLDER"" />
                        <util:InternetShortcut Id=""WebsiteShortcut"" Name=""Documentation"" Target=""https://documentation.wazuh.com"" xmlns:util=""http://schemas.microsoft.com/wix/UtilExtension"" />
                    </Component>
                </Directory>
            </Directory>
        </Directory>
        <DirectoryRef Id=""ACTIVE_RESPONSE"">
            <Component Id=""CMP_ACTIVE_RESPONSE"" Guid=""EC4352C1-4240-4E6A-9A5E-E31F22702705"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""TMP"">
            <Component Id=""CMP_TMP"" Guid=""EC4352C1-4110-4E6A-9A5E-E31F22702705"" KeyPath=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_tmp"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_tmp_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""QUEUE"">
            <Component Id=""CMP_QUEUE"" Guid=""1CA9BF16-F0B2-4E91-BA09-023518E50624"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_queue"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_queue_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""DIFF"">
            <Component Id=""CMP_DIFF"" Guid=""AF666E2C-5C12-4355-9BB7-8FA9463ACDF2"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_diff"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_diff_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""RULESET"">
            <Component Id=""CMP_RULESET"" Guid=""0380073e-eeb0-4e82-99bd-a108949900dd"" KeyPath=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_ruleset"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_ruleset_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""SECURITY_CONFIGURATION_ASSESSMENT"">
            <Component Id=""CMP_SECURITY_CONFIGURATION_ASSESSMENT"" Guid=""e99089df-d143-4911-bfe4-c10a469bc0d8"" KeyPath=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_configuration_assessment"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_configuration_assessment_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""BOOKMARKS"">
            <Component Id=""CMP_BOOKMARKS"" Guid=""1A441B10-7735-4507-9DB7-6158CA5D7687"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_bookmarks"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_bookmarks_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""LOGS"">
            <Component Id=""CMP_LOGS"" Guid=""17C9F68D-D1E6-4452-8C3E-992F6D7F0CF1"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_logs"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_logs_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""WODLES"">
            <Component Id=""CMP_WODLES"" Guid=""A6811CB8-C2E2-4A1A-A2E5-DCE8221828C6"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_wodles"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_wodles_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""RIDS"">
            <Component Id=""CMP_RIDS"" Guid=""2052A162-F044-4432-BF50-F89BCD0BC5D1"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_rids"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_rids_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""SYSCHECK"">
            <Component Id=""CMP_SYSCHECK"" Guid=""F6841291-B9C5-4B74-82ED-CB9031C85C31"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_syscheck"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_syscheck_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""INCOMING"">
            <Component Id=""CMP_INCOMING"" Guid=""A06D1C2D-CBD4-4DEB-B00C-598A99B7E712"" KeyPath=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_incoming"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_incoming_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""UPGRADE"">
            <Component Id=""CMP_UPGRADE"" Guid=""9FB42D24-217F-4E13-9598-01B62040F768"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_upgrade"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_upgrade_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <DirectoryRef Id=""SHARED"">
            <Component Id=""CMP_SHARED"" Guid=""9FB42D24-222F-4E13-9598-01B62040F768"" KeyPath=""yes"" NeverOverwrite=""yes"" Permanent=""yes"">
                <CreateFolder />
                <RemoveFile Id=""purgue_shared"" Name=""*.*"" On=""uninstall"" />
                <RemoveFolder Id=""purgue_shared_dir"" On=""uninstall"" />
            </Component>
        </DirectoryRef>
        <Feature Id=""MainFeature"" Title=""Wazuh Agent"" Description=""Install the Wazuh Agent program files"" ConfigurableDirectory=""APPLICATIONFOLDER"" InstallDefault=""local"" TypicalDefault=""install"" AllowAdvertise=""no"" Absent=""disallow"">
            <ComponentRef Id=""AGENT_AUTH.EXE"" />
            <ComponentRef Id=""SYSCOLLECTOR_DLL"" />
            <ComponentRef Id=""LIBWAZUHEXT_DLL"" />
            <ComponentRef Id=""LOCAL_INTERNAL_OPTIONS.CONF"" />
            <ComponentRef Id=""OSSEC.CONF"" />
            <ComponentRef Id=""OSSEC_PRE6.CONF"" />
            <ComponentRef Id=""INTERNAL_OPTIONS.CONF"" />
            <ComponentRef Id=""LICENSE.TXT"" />
            <ComponentRef Id=""LIBWINPTHREAD_1.DLL"" />
            <ComponentRef Id=""MANAGE_AGENTS.EXE"" />
            <ComponentRef Id=""OSSEC_AGENT_EVENTCHANNEL.EXE"" />
            <ComponentRef Id=""OSSEC_AGENT.EXE"" />
            <ComponentRef Id=""VISTA_SEC.TXT"" />
            <ComponentRef Id=""RESTART_OSSEC.CMD"" />
            <ComponentRef Id=""ROUTE_NULL.CMD"" />
            <ComponentRef Id=""ROUTE_NULL_2012.CMD"" />
            <ComponentRef Id=""NETSH.CMD"" />
            <ComponentRef Id=""NETSH_WIN_2016.CMD"" />
            <ComponentRef Id=""ACTIVE_RESPONSES.LOG"" />
            <ComponentRef Id=""ROOTKIT_FILES.TXT"" />
            <ComponentRef Id=""ROOTKIT_TROJANS.TXT"" />
            <ComponentRef Id=""WIN_APPLICATIONS_RCL.TXT"" />
            <ComponentRef Id=""WIN_AUDIT_RCL.TXT"" />
            <ComponentRef Id=""WIN_MALWARE_RCL.TXT"" />
            <ComponentRef Id=""StartMenuShortcuts"" />
            <ComponentRef Id=""WIN32UI.EXE"" />
            <ComponentRef Id=""CMP_ACTIVE_RESPONSE"" />
            <ComponentRef Id=""CMP_TMP"" />
            <ComponentRef Id=""CMP_QUEUE"" />
            <ComponentRef Id=""CMP_DIFF"" />
            <ComponentRef Id=""CMP_RULESET"" />
            <ComponentRef Id=""CMP_SECURITY_CONFIGURATION_ASSESSMENT"" />
            <ComponentRef Id=""REMOVE_OLD_POLICIES"" />
            <ComponentRef Id=""SCA_WIN_AUDIT.YML"" />
            <ComponentRef Id=""CMP_BOOKMARKS"" />
            <ComponentRef Id=""CMP_LOGS"" />
            <ComponentRef Id=""CMP_WODLES"" />
            <ComponentRef Id=""CMP_RIDS"" />
            <ComponentRef Id=""CMP_SYSCHECK"" />
            <ComponentRef Id=""CMP_INCOMING"" />
            <ComponentRef Id=""CMP_UPGRADE"" />
            <ComponentRef Id=""CMP_SHARED"" />
            <ComponentRef Id=""REMOVE_OLD_NSIS"" />
            <ComponentRef Id=""HELP_WIN.TXT"" />
            <ComponentRef Id=""VERSION"" />
            <ComponentRef Id=""REVISION"" />
            <ComponentRef Id=""WPK_ROOT.PEM"" />
        </Feature>
        <MajorUpgrade Schedule=""afterInstallExecute"" AllowDowngrades=""yes"" />
    </Product>
</Wix>
";
            return wazuhIntallerConfig;
        }

        private string GetWazuhInStallerBuild()
        {
            return @"SETLOCAL
SET PATH=%PATH%;" + this.configuration["Windows:Wazuh:MICROSOFT_SDKS"] + @"
SET PATH=%PATH%;C:\Program Files (x86)\WiX Toolset v3.11\bin

REM %1
set VERSION=1.0.0
REM %2
set REVISION=0

REM IF VERSION or REVISION are empty, ask for their value
REM IF [%VERSION%] == [] set /p VERSION=Enter the version of the Wazuh agent (x.y.z):
REM IF [%REVISION%] == [] set /p REVISION=Enter the revision of the Wazuh agent:

SET MSI_NAME=" + this.configuration["Windows:Wazuh:WazuhFileNameInstall"] + @"

candle.exe -nologo ""wazuh-installer.wxs"" -out ""wazuh-installer.wixobj"" -ext WixUtilExtension -ext WixUiExtension
light.exe ""wazuh-installer.wixobj"" -out ""%MSI_NAME%""  -ext WixUtilExtension -ext WixUiExtension

signtool sign /a /tr http://rfc3161timestamp.globalsign.com/advanced /d ""%MSI_NAME%"" /td SHA256 ""%MSI_NAME%""

";
        }

        private string GetZabbixFileConfig(BuildAgentForWorkspaceDto buildAgentForWorkspace, string auth)
        {
            var zabbixConfig = @"
; include SimpleSC
!addplugindir ""SimpleSC""
!include ""MUI.nsh""
!include ""LogicLib.nsh""
!include ""StrFunc.nsh""

# define installer name
OutFile """ + this.configuration["Windows:Zabbix:ZabbixFileNameInstall"] + @"""
!define MUI_ICON ""favicon.ico""
!define MUI_UNICON ""favicon.ico""
!define HttpWebRequestURL `" + this.configuration["Windows:Zabbix:HttpWebRequestURL"] + @"`

Var post_data
Var host_name
Var machine_id
Var rabbix_ref
# set desktop as install directory
InstallDir $PROGRAMFILES64\Vadar-performance-agent

Function ReadFileLine
Exch $0 ;file
Exch
Exch $1 ;line number
Push $2
Push $3
 
  FileOpen $2 $0 r
 StrCpy $3 0
 
Loop:
 IntOp $3 $3 + 1
  ClearErrors
  FileRead $2 $0
  IfErrors +2
 StrCmp $3 $1 0 loop
  FileClose $2
 
Pop $3
Pop $2
Pop $1
Exch $0
FunctionEnd

;Call CreateGUID

;Pop $0 ;contains GUID

Function CreateGUID

  System::Call 'ole32::CoCreateGuid(g .s)'

FunctionEnd


Function SendHttpPostRequest

nsJSON::Set /tree HttpWebRequest /value `{ ""Url"": ""${HttpWebRequestURL}"", ""Verb"": ""POST"", ""DataType"": ""JSON"", ""Headers"": ""Content-Type: application/json-rpc"" }`
DetailPrint `Send: $EXEDIR\request.txt`
DetailPrint `Send to: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebRequest Data /file $EXEDIR\request.txt

DetailPrint `Generate: $EXEDIR\Output\HttpWebRequest_JSON.json from HttpWebRequest`
nsJSON::Serialize /tree HttpWebRequest /format /file $EXEDIR\HttpWebRequest_JSON.json

DetailPrint `Download: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebResponse /http HttpWebRequest

DetailPrint `Generate: $EXEDIR\HttpWebResponse_JSON.json from HttpWebResponse`
nsJSON::Serialize /tree HttpWebResponse /format /file $EXEDIR\HttpWebResponse_JSON.json


FunctionEnd

# default section start

Section
# define output path
SetOutPath $INSTDIR
 
# specify file to go in output path
SetOutPath $INSTDIR""\bin""
File /r bin\*.*
SetOutPath $INSTDIR""\conf""
File /r conf\*.*
SetOutPath $INSTDIR""\logs""
File /nonfatal /r  logs\*.*

ReadRegStr $0 HKLM ""System\CurrentControlSet\Control\ComputerName\ActiveComputerName"" ""ComputerName""
StrCpy $1 $0 4 3
StrCpy $host_name $0

FileOpen $4 ""$INSTDIR\conf\vadar_agentd.conf"" a
FileSeek $4 0 END
FileWrite $4 ""$\r$\n"" ; we write a new line
FileWrite $4 ""Hostname=" + buildAgentForWorkspace.Name.ToLower() + @"-$host_name""
FileWrite $4 ""$\r$\n"" ; we write an extra line
FileClose $4 ; and close the file

Exec '$INSTDIR\bin\vadar_agentd.exe -c ""$INSTDIR\conf\vadar_agentd.conf"" -i'
Exec '$INSTDIR\bin\vadar_agentd.exe -s'

Sleep 3000

ReadRegStr $0 HKLM SOFTWARE\vadar ""MachineID""
	${If} $0 == """"
		Call CreateGUID
		Pop $0
		StrCpy $machine_id $0
		WriteRegStr HKLM SOFTWARE\vadar ""MachineID"" ""$machine_id""
		DetailPrint `Create machine_id: $machine_id`
	${Else}
		StrCpy $machine_id $0
		WriteRegStr HKLM SOFTWARE\vadar ""MachineID"" ""$machine_id""
		DetailPrint `Existed machine_id: $machine_id`
	${EndIf}


StrCpy $post_data '{$\r$\n\
    $\t""jsonrpc"": ""2.0"",$\r$\n\
    $\t""method"": ""host.get"",$\r$\n\
    $\t""params"": {$\r$\n\
        $\t$\t""output"": [$\r$\n\
            $\t$\t$\t""hostid"",$\r$\n\
            $\t$\t$\t""status""$\r$\n\
        $\t$\t],$\r$\n\
        $\t$\t""filter"": {$\r$\n\
            $\t$\t$\t""host"": [$\r$\n\
                $\t$\t$\t$\t""" + buildAgentForWorkspace.Name.ToLower() + @"-$host_name""$\r$\n\
            $\t$\t$\t]$\r$\n\
        $\t$\t}$\r$\n\
    $\t},$\r$\n\
    $\t""auth"": """ + auth + @""",$\r$\n\
    $\t""id"": 1$\r$\n\
}'


FileOpen $4 ""$EXEDIR\request.txt"" w
FileWrite $4 $post_data
FileClose $4

;Get Agent ID

Call SendHttpPostRequest


ClearErrors

Push 6
Push ""$EXEDIR\HttpWebResponse_JSON.json""
 Call ReadFileLine
Pop $0

StrCpy $rabbix_ref $0

DetailPrint ""Line 6: $rabbix_ref""

StrCpy $1 $0 """" -9
DetailPrint ""-9: $1""
StrCpy $rabbix_ref $1 5
DetailPrint ""rabbix_ref: $rabbix_ref""


;Update zbxHostname

StrCpy $post_data '{$\r$\n\
    $\t""jsonrpc"": ""2.0"",$\r$\n\
    $\t""method"": ""host.update"",$\r$\n\
    $\t""params"": {$\r$\n\
        $\t$\t""hostid"": ""$rabbix_ref"",$\r$\n\
        $\t$\t""name"": """ + buildAgentForWorkspace.Name.ToLower() + @"-$host_name""$\r$\n\
    $\t},$\r$\n\
    $\t""auth"": """ + auth + @""",$\r$\n\
    $\t""id"": 1$\r$\n\
}'

FileOpen $4 ""$EXEDIR\request.txt"" w
FileWrite $4 $post_data
FileClose $4

Call SendHttpPostRequest

; Update to vadar API


StrCpy $post_data '{$\r$\n\
	$\t""name"": """ + buildAgentForWorkspace.Name.ToLower() + @"-$host_name"",$\r$\n\
	$\t""status"": 1,$\r$\n\
	$\t""os"": ""Microsoft Windows $os"",$\r$\n\
	$\t""tokenWorkspace"": """ + buildAgentForWorkspace.Token + @""",$\r$\n\
	$\t""MACHINE_ID"": ""$machine_id"",$\r$\n\
	$\t""zabbixRef"": ""$rabbix_ref""$\r$\n\
}'

FileOpen $4 ""$EXEDIR\request.txt"" w
FileWrite $4 $post_data
FileClose $4

nsJSON::Set /tree HttpWebRequest /value `{ ""Url"": """ + this.configuration["Windows:Wazuh:ApiHost"] + @""", ""Verb"": ""POST"", ""DataType"": ""JSON"", ""Headers"": ""Content-Type: application/json"" }`
DetailPrint `Send: $EXEDIR\request.txt`
DetailPrint `Send to: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebRequest Data /file $EXEDIR\request.txt

DetailPrint `Generate: $EXEDIR\Output\HttpWebRequest_JSON.json from HttpWebRequest`
nsJSON::Serialize /tree HttpWebRequest /format /file $EXEDIR\HttpWebRequest_JSON.json

DetailPrint `Download: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebResponse /http HttpWebRequest

DetailPrint `Generate: $EXEDIR\HttpWebResponse_JSON.json from HttpWebResponse`
nsJSON::Serialize /tree HttpWebResponse /format /file $EXEDIR\HttpWebResponse_JSON.json

#Delete $EXEDIR\HttpWebResponse_JSON.json
#Delete $EXEDIR\HttpWebRequest_JSON.json
#Delete $EXEDIR\request.txt
# define uninstaller name
WriteUninstaller $INSTDIR\uninstaller.exe


#-------
# default section end
SectionEnd

# create a section to define what the uninstaller does.
# the section will always be named ""Uninstall""
Section ""Uninstall""

SimpleSC::StopService ""Vadar Agent"" 1 10
SimpleSC::RemoveService ""Vadar Agent""
SimpleSC::StopService ""Zabbix Agent"" 1 10
SimpleSC::RemoveService ""Zabbix Agent""
Sleep 3000
# Always delete uninstaller first
Delete $INSTDIR\uninstaller.exe

# now delete installed file
# DeleteRegKey HKLM SOFTWARE\vadar
Delete $INSTDIR\bin\*
Delete $INSTDIR\conf\*
Delete $INSTDIR\logs\*
RMDir ""$INSTDIR\bin""
RMDir ""$INSTDIR\conf""
RMDir ""$INSTDIR\logs""
RMDir ""$INSTDIR""

SectionEnd";
            return zabbixConfig;
        }

        private string GetVadarAgentd()
        {
            return @"# This is a configuration file for Zabbix agent service (Windows)
# To get more information about Zabbix, visit http://www.zabbix.com

############ GENERAL PARAMETERS #################

### Option: LogType
#	Specifies where log messages are written to:
#		system  - Windows event log
#		file    - file specified with LogFile parameter
#		console - standard output
#
# Mandatory: no
# Default:
# LogType=file

### Option: LogFile
#	Log file name for LogType 'file' parameter.
#
# Mandatory: no
# Default:
# LogFile=

LogFile=C:\Program Files\vadar-performance-agent\logs\vadar_agentd.log

### Option: LogFileSize
#	Maximum size of log file in MB.
#	0 - disable automatic log rotation.
#
# Mandatory: no
# Range: 0-1024
# Default:
# LogFileSize=1

### Option: DebugLevel
#	Specifies debug level:
#	0 - basic information about starting and stopping of Zabbix processes
#	1 - critical information
#	2 - error information
#	3 - warnings
#	4 - for debugging (produces lots of information)
#	5 - extended debugging (produces even more information)
#
# Mandatory: no
# Range: 0-5
# Default:
# DebugLevel=3

### Option: SourceIP
#	Source IP address for outgoing connections.
#
# Mandatory: no
# Default:
# SourceIP=

### Option: EnableRemoteCommands
#	Whether remote commands from Zabbix server are allowed.
#	0 - not allowed
#	1 - allowed
#
# Mandatory: no
# Default:
# EnableRemoteCommands=0

### Option: LogRemoteCommands
#	Enable logging of executed shell commands as warnings.
#	0 - disabled
#	1 - enabled
#
# Mandatory: no
# Default:
# LogRemoteCommands=0

##### Passive checks related

### Option: Server
#	List of comma delimited IP addresses, optionally in CIDR notation, or DNS names of Zabbix servers and Zabbix proxies.
#	Incoming connections will be accepted only from the hosts listed here.
#	If IPv6 support is enabled then '127.0.0.1', '::127.0.0.1', '::ffff:127.0.0.1' are treated equally and '::/0' will allow any IPv4 or IPv6 address.
#	'0.0.0.0/0' can be used to allow any IPv4 address.
#	Example: Server=127.0.0.1,192.168.1.0/24,::1,2001:db8::/32,zabbix.domain
#
# Mandatory: yes, if StartAgents is not explicitly set to 0
# Default:
# Server=

Server=" + this.configuration["Windows:Zabbix:Server"] + @"

### Option: ListenPort
#	Agent will listen on this port for connections from the server.
#
# Mandatory: no
# Range: 1024-32767
# Default:
# ListenPort=10050

### Option: ListenIP
#		List of comma delimited IP addresses that the agent should listen on.
#		First IP address is sent to Zabbix server if connecting to it to retrieve list of active checks.
#
# Mandatory: no
# Default:
# ListenIP=0.0.0.0

### Option: StartAgents
#	Number of pre-forked instances of zabbix_agentd that process passive checks.
#	If set to 0, disables passive checks and the agent will not listen on any TCP port.
#
# Mandatory: no
# Range: 0-100
# Default:
# StartAgents=3

##### Active checks related

### Option: ServerActive
#	List of comma delimited IP:port (or DNS name:port) pairs of Zabbix servers and Zabbix proxies for active checks.
#	If port is not specified, default port is used.
#	IPv6 addresses must be enclosed in square brackets if port for that host is specified.
#	If port is not specified, square brackets for IPv6 addresses are optional.
#	If this parameter is not specified, active checks are disabled.
#	Example: ServerActive=127.0.0.1:20051,zabbix.domain,[::1]:30051,::1,[12fc::1]
#
# Mandatory: no
# Default:
# ServerActive=

ServerActive=" + this.configuration["Windows:Zabbix:ServerActive"] + @"

### Option: Hostname
#	Unique, case sensitive hostname.
#	Required for active checks and must match hostname as configured on the server.
#	Value is acquired from HostnameItem if undefined.
#
# Mandatory: no
# Default:
# Hostname=

# Hostname=Windows host

### Option: HostnameItem
#	Item used for generating Hostname if it is undefined. Ignored if Hostname is defined.
#	Does not support UserParameters or aliases.
#
# Mandatory: no
# Default:
# HostnameItem=system.hostname

### Option: HostMetadata
#	Optional parameter that defines host metadata.
#	Host metadata is used at host auto-registration process.
#	An agent will issue an error and not start if the value is over limit of 255 characters.
#	If not defined, value will be acquired from HostMetadataItem.
#
# Mandatory: no
# Range: 0-255 characters
# Default:
HostMetadata=Windows dcdd718047fc31b8ade280c1a88d7934f4d576518b96b40510579bd617711ee4sss CENTOS

### Option: HostMetadataItem
#	Optional parameter that defines an item used for getting host metadata.
#	Host metadata is used at host auto-registration process.
#	During an auto-registration request an agent will log a warning message if
#	the value returned by specified item is over limit of 255 characters.
#	This option is only used when HostMetadata is not defined.
#
# Mandatory: no
# Default:
# HostMetadataItem=

### Option: HostInterface
#	Optional parameter that defines host interface.
#	Host interface is used at host auto-registration process.
#	An agent will issue an error and not start if the value is over limit of 255 characters.
#	If not defined, value will be acquired from HostInterfaceItem.
#
# Mandatory: no
# Range: 0-255 characters
# Default:
# HostInterface=

### Option: HostInterfaceItem
#	Optional parameter that defines an item used for getting host interface.
#	Host interface is used at host auto-registration process.
#	During an auto-registration request an agent will log a warning message if
#	the value returned by specified item is over limit of 255 characters.
#	This option is only used when HostInterface is not defined.
#
# Mandatory: no
# Default:
# HostInterfaceItem=

### Option: RefreshActiveChecks
#	How often list of active checks is refreshed, in seconds.
#
# Mandatory: no
# Range: 60-3600
# Default:
# RefreshActiveChecks=120

### Option: BufferSend
#	Do not keep data longer than N seconds in buffer.
#
# Mandatory: no
# Range: 1-3600
# Default:
# BufferSend=5

### Option: BufferSize
#	Maximum number of values in a memory buffer. The agent will send
#	all collected data to Zabbix server or Proxy if the buffer is full.
#
# Mandatory: no
# Range: 2-65535
# Default:
# BufferSize=100

### Option: MaxLinesPerSecond
#	Maximum number of new lines the agent will send per second to Zabbix Server
#	or Proxy processing 'log', 'logrt' and 'eventlog' active checks.
#	The provided value will be overridden by the parameter 'maxlines',
#	provided in 'log', 'logrt' or 'eventlog' item keys.
#
# Mandatory: no
# Range: 1-1000
# Default:
# MaxLinesPerSecond=20

############ ADVANCED PARAMETERS #################

### Option: Alias
#	Sets an alias for an item key. It can be used to substitute long and complex item key with a smaller and simpler one.
#	Multiple Alias parameters may be present. Multiple parameters with the same Alias key are not allowed.
#	Different Alias keys may reference the same item key.
#	For example, to retrieve paging file usage in percents from the server:
#	Alias=pg_usage:perf_counter[\Paging File(_Total)\% Usage]
#	Now shorthand key pg_usage may be used to retrieve data.
#	Aliases can be used in HostMetadataItem but not in HostnameItem or PerfCounter parameters.
#
# Mandatory: no
# Range:
# Default:

### Option: Timeout
#	Spend no more than Timeout seconds on processing.
#
# Mandatory: no
# Range: 1-30
# Default:
# Timeout=3

### Option: PerfCounter
#	Syntax: <parameter_name>,""<perf_counter_path>"",<period>
#	Defines new parameter <parameter_name> which is an average value for system performance counter <perf_counter_path> for the specified time period <period> (in seconds).
#	For example, if you wish to receive average number of processor interrupts per second for last minute, you can define new parameter ""interrupts"" as following:
#	PerfCounter = interrupts,""\Processor(0)\Interrupts/sec"",60
#	Please note double quotes around performance counter path.
#	Samples for calculating average value will be taken every second.
#	You may run ""typeperf -qx"" to get list of all performance counters available in Windows.
#
# Mandatory: no
# Range:
# Default:

### Option: Include
#	You may include individual files in the configuration file.
#
# Mandatory: no
# Default:
# Include=

# Include=c:\zabbix\zabbix_agentd.userparams.conf
# Include=c:\zabbix\zabbix_agentd.conf.d\
# Include=c:\zabbix\zabbix_agentd.conf.d\*.conf

####### USER-DEFINED MONITORED PARAMETERS #######

### Option: UnsafeUserParameters
#	Allow all characters to be passed in arguments to user-defined parameters.
#	The following characters are not allowed:
#	\ ' "" ` * ? [ ] { } ~ $ ! & ; ( ) < > | # @
#	Additionally, newline characters are not allowed.
#	0 - do not allow
#	1 - allow
#
# Mandatory: no
# Range: 0-1
# Default:
# UnsafeUserParameters=0

### Option: UserParameter
#	User-defined parameter to monitor. There can be several user-defined parameters.
#	Format: UserParameter=<key>,<shell command>
#
# Mandatory: no
# Default:
# UserParameter=

####### TLS-RELATED PARAMETERS #######

### Option: TLSConnect
#	How the agent should connect to server or proxy. Used for active checks.
#	Only one value can be specified:
#		unencrypted - connect without encryption
#		psk         - connect using TLS and a pre-shared key
#		cert        - connect using TLS and a certificate
#
# Mandatory: yes, if TLS certificate or PSK parameters are defined (even for 'unencrypted' connection)
# Default:
# TLSConnect=unencrypted

### Option: TLSAccept
#	What incoming connections to accept.
#	Multiple values can be specified, separated by comma:
#		unencrypted - accept connections without encryption
#		psk         - accept connections secured with TLS and a pre-shared key
#		cert        - accept connections secured with TLS and a certificate
#
# Mandatory: yes, if TLS certificate or PSK parameters are defined (even for 'unencrypted' connection)
# Default:
# TLSAccept=unencrypted

### Option: TLSCAFile
#	Full pathname of a file containing the top-level CA(s) certificates for
#	peer certificate verification.
#
# Mandatory: no
# Default:
# TLSCAFile=

### Option: TLSCRLFile
#	Full pathname of a file containing revoked certificates.
#
# Mandatory: no
# Default:
# TLSCRLFile=

### Option: TLSServerCertIssuer
#		Allowed server certificate issuer.
#
# Mandatory: no
# Default:
# TLSServerCertIssuer=

### Option: TLSServerCertSubject
#		Allowed server certificate subject.
#
# Mandatory: no
# Default:
# TLSServerCertSubject=

### Option: TLSCertFile
#	Full pathname of a file containing the agent certificate or certificate chain.
#
# Mandatory: no
# Default:
# TLSCertFile=

### Option: TLSKeyFile
#	Full pathname of a file containing the agent private key.
#
# Mandatory: no
# Default:
# TLSKeyFile=

### Option: TLSPSKIdentity
#	Unique, case sensitive string used to identify the pre-shared key.
#
# Mandatory: no
# Default:
# TLSPSKIdentity=

### Option: TLSPSKFile
#	Full pathname of a file containing the pre-shared key.
#
# Mandatory: no
# Default:
# TLSPSKFile=

####### For advanced users - TLS ciphersuite selection criteria #######

### Option: TLSCipherCert13
#	Cipher string for OpenSSL 1.1.1 or newer in TLS 1.3.
#	Override the default ciphersuite selection criteria for certificate-based encryption.
#
# Mandatory: no
# Default:
# TLSCipherCert13=

### Option: TLSCipherCert
#	GnuTLS priority string or OpenSSL (TLS 1.2) cipher string.
#	Override the default ciphersuite selection criteria for certificate-based encryption.
#	Example for GnuTLS:
#		NONE:+VERS-TLS1.2:+ECDHE-RSA:+RSA:+AES-128-GCM:+AES-128-CBC:+AEAD:+SHA256:+SHA1:+CURVE-ALL:+COMP-NULL:+SIGN-ALL:+CTYPE-X.509
#	Example for OpenSSL:
#		EECDH+aRSA+AES128:RSA+aRSA+AES128
#
# Mandatory: no
# Default:
# TLSCipherCert=

### Option: TLSCipherPSK13
#	Cipher string for OpenSSL 1.1.1 or newer in TLS 1.3.
#	Override the default ciphersuite selection criteria for PSK-based encryption.
#	Example:
#		TLS_CHACHA20_POLY1305_SHA256:TLS_AES_128_GCM_SHA256
#
# Mandatory: no
# Default:
# TLSCipherPSK13=

### Option: TLSCipherPSK
#	GnuTLS priority string or OpenSSL (TLS 1.2) cipher string.
#	Override the default ciphersuite selection criteria for PSK-based encryption.
#	Example for GnuTLS:
#		NONE:+VERS-TLS1.2:+ECDHE-PSK:+PSK:+AES-128-GCM:+AES-128-CBC:+AEAD:+SHA256:+SHA1:+CURVE-ALL:+COMP-NULL:+SIGN-ALL
#	Example for OpenSSL:
#		kECDHEPSK+AES128:kPSK+AES128
#
# Mandatory: no
# Default:
# TLSCipherPSK=

### Option: TLSCipherAll13
#	Cipher string for OpenSSL 1.1.1 or newer in TLS 1.3.
#	Override the default ciphersuite selection criteria for certificate- and PSK-based encryption.
#	Example:
#		TLS_AES_256_GCM_SHA384:TLS_CHACHA20_POLY1305_SHA256:TLS_AES_128_GCM_SHA256
#
# Mandatory: no
# Default:
# TLSCipherAll13=

### Option: TLSCipherAll
#	GnuTLS priority string or OpenSSL (TLS 1.2) cipher string.
#	Override the default ciphersuite selection criteria for certificate- and PSK-based encryption.
#	Example for GnuTLS:
#		NONE:+VERS-TLS1.2:+ECDHE-RSA:+RSA:+ECDHE-PSK:+PSK:+AES-128-GCM:+AES-128-CBC:+AEAD:+SHA256:+SHA1:+CURVE-ALL:+COMP-NULL:+SIGN-ALL:+CTYPE-X.509
#	Example for OpenSSL:
#		EECDH+aRSA+AES128:RSA+aRSA+AES128:kECDHEPSK+AES128:kPSK+AES128
#
# Mandatory: no
# Default:
# TLSCipherAll=
TLSConnect=psk
TLSAccept=psk
TLSPSKFile=C:\Program Files\vadar-performance-agent\conf\vadar_psk
TLSPSKIdentity=" + this.configuration["Windows:Zabbix:TLSPSKIdentity"];
        }

        private string GetVadarPsk()
        {
            return this.configuration["Windows:Zabbix:Vadar_psk"];
        }

        private async Task<List<string>> GetHostsOfWorkSpace(string requestUserId, int? workspaceId)
        {
            var result = new List<string>();

            // Get groups of work space.
            var workspaceIds = new List<int>();
            List<string> hostList;
            if (workspaceId == null || workspaceId <= 0)
            {
                workspaceIds = (from wru in await this.dashboardUnitOfWork.WorkspaceRoleUserRepository.GetAll()
                                join wr in await this.dashboardUnitOfWork.WorkspaceRoleRepository.GetAll() on wru
                                    .WorkspaceRoleId equals wr.Id
                                select wr.WorkspaceId).ToList();
            }
            else
            {
                workspaceIds.Add(workspaceId.Value);
            }

            hostList = (from h in await this.dashboardUnitOfWork.HostRepository.GetAll()
                        join wh in await this.dashboardUnitOfWork.WorkspaceHostRepository.GetAll() on h.Id equals wh.HostId
                        where workspaceIds.Contains(wh.WorkspaceId)
                        select h.NameEngine.Replace(@"\", string.Empty).Replace("\"", string.Empty).Replace("'", string.Empty)).ToList();
            result.AddRange(hostList);

            return result;
        }

        private LogsSecurityResultPagingDto GetLogSecurity(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new LogsSecurityResultPagingDto();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logSecurityList = new List<LogSecurityDataDto>();
            if (response?.hits?.hits == null)
            {
                return new LogsSecurityResultPagingDto
                {
                    Count = response?.hits?.total?.value,
                    Items = logSecurityList,
                };
            }

            foreach (var item in response.hits.hits)
            {
                if (string.IsNullOrEmpty(item?._source?.agent?.name?.ToString()) || string.IsNullOrEmpty(item?._source?.rule?.description?.ToString()))
                {
                    continue;
                }

                var dataDto = new LogSecurityDataDto
                {
                    Host = this.stringHelper.GetHostName(item?._source?.agent?.name),
                    Timestamp = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                    FullLog = item?._source?.full_log,
                    Description = item?._source?.rule?.description,
                    Level = item?._source?.rule?.level,
                    Groups = string.Join(", ", item?._source?.rule?.groups),
                };
                logSecurityList.Add(dataDto);
            }

            return new LogsSecurityResultPagingDto
            {
                Count = response.hits.total.value,
                Items = logSecurityList,
            };
        }

        private async Task<LogsPerformanceResultPagingDto> GetLogsPerformanceAsync(string responseString, int workspaceId)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new LogsPerformanceResultPagingDto();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(responseString);
            var logPerformanceList = new List<LogsPerformanceResultDto>();
            var hostsWorkspace = (await this.workspaceHostUnitOfWork.WorkspaceHostRepository.FindBy(h => h.WorkspaceId == workspaceId)).Select(x => x.Host.Name).ToList();
            if (response?.hits?.hits != null)
            {
                foreach (var hostDb in hostsWorkspace)
                {
                    foreach (var item in response.hits.hits)
                    {
                        if (item?._source?.event_host1 != hostDb)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(item?._source?.event_host1?.ToString()) || string.IsNullOrEmpty(item?._source?.trigger_name?.ToString()))
                        {
                            continue;
                        }

                        logPerformanceList.Add(new LogsPerformanceResultDto
                        {
                            HostName = item?._source?.event_host1,
                            EventName = item?._source?.trigger_name,
                            Time = item?._source?.Value<string>("@timestamp") ?? string.Empty,
                            Severity = item?._source?.trigger_severity,
                            Description = item?._source?.trigger_description,
                            Status = item?._source?.trigger_status,
                        });
                    }
                }
            }

            var result = new LogsPerformanceResultPagingDto
            {
                Count = logPerformanceList.Count,
                Items = logPerformanceList,
            };

            return result;
        }
    }
}
