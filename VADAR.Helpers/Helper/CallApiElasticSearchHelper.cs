// <copyright file="CallApiElasticSearchHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Helpers.Interfaces;
using VADAR.Helpers.Utilities;
using static VADAR.Helpers.Const.Constants;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Call Api ElasticSearch Helper.
    /// </summary>
    public class CallApiElasticSearchHelper : IElasticSearchCallApiHelper
    {
        private readonly IConfiguration configs;
        private readonly ILoggerHelper<CallApiElasticSearchHelper> logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="CallApiElasticSearchHelper"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="configs">configs.</param>
        public CallApiElasticSearchHelper(ILoggerHelper<CallApiElasticSearchHelper> logger, IConfiguration configs)
        {
            this.configs = configs;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateNotification(NotificationDto notificationDto)
        {
            notificationDto.Id = Guid.NewGuid();
            var content = JsonConvert.SerializeObject(notificationDto);
            var index = this.configs["Elasticsearch:Notification"];

            var result = await this.AddNewRecord(content, notificationDto.Id, index);

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateNotificationError(string body)
        {
            var id = Guid.NewGuid();
            var content = JsonConvert.SerializeObject(new { id = id, content = body });
            var index = this.configs["Elasticsearch:NotificationError"];

            var result = await this.AddNewRecord(content, id, index);

            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetNetworkLog(LogsNetworkRequestDto logsNetworkRequestDto)
        {
            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (logsNetworkRequestDto.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", logsNetworkRequestDto.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (!string.IsNullOrEmpty(logsNetworkRequestDto.HostName))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("agent.name", new List<string> { logsNetworkRequestDto.HostName }));
            }

            if (!string.IsNullOrEmpty(logsNetworkRequestDto.SourceAddress))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("data.src_addr", new List<string> { logsNetworkRequestDto.SourceAddress }));
            }

            if (!string.IsNullOrEmpty(logsNetworkRequestDto.DestinationAddress))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("data.dst_addr", new List<string> { logsNetworkRequestDto.DestinationAddress }));
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
  ""sort"": [
    {
      ""@timestamp"": {
        ""order"": ""desc"",
        ""unmapped_type"": ""boolean""
      }
    }
  ],
""from"": """ + ((logsNetworkRequestDto.PageIndex - 1) * logsNetworkRequestDto.PageSize) + @""",
""size"": """ + logsNetworkRequestDto.PageSize + @""",
   ""_source"":{
      ""includes"":[
      ""@timestamp"",
      ""agent.name"",
      ""agent.group"",
      ""data.action"",
      ""data.class"",
      ""data.src_addr"",
      ""data.src_port"",
      ""data.dst_port"",
      ""data.dst_addr"",
      ""data.msg"",
      ""rule.description""
      ],
      ""excludes"":[
      ]
   },
   ""query"":{
      ""bool"":{
         ""must"":[{
            ""match"": {
                        ""rule.description"": ""Snort Message""
                      }
            }],
         ""filter"":[" +
                  strQueryConditions
                + (logsNetworkRequestDto.FromDate.HasValue || logsNetworkRequestDto.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (logsNetworkRequestDto.FromDate.HasValue ? @"""gte"":""" + logsNetworkRequestDto.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (logsNetworkRequestDto.ToDate.HasValue ? @"""lte"":""" + logsNetworkRequestDto.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"]
      }
   }
}";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetPerformanceLog(LogsPerformanceRequestDto logsPerformanceRequest)
        {
            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/zabbix-problem-2-*/_search";

            var lstQueryCondition = new List<string>();

            if (!string.IsNullOrEmpty(logsPerformanceRequest.Severity))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("trigger_severity", new List<string> { logsPerformanceRequest.Severity }));
            }

            if (!string.IsNullOrEmpty(logsPerformanceRequest.Status))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("trigger_status", new List<string> { logsPerformanceRequest.Status }));
            }

            if (logsPerformanceRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterMonitor("event_name1", logsPerformanceRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (!string.IsNullOrEmpty(logsPerformanceRequest.GroupName))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("trigger_hostgroup_name", new List<string> { logsPerformanceRequest.GroupName }));
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            using var client = new HttpClient();

            var body1 = @"{
                          ""version"": true,
                          ""sort"": [
                            {
                              ""@timestamp"": {
                                ""order"": ""desc"",
                                ""unmapped_type"": ""boolean""
                              }
                            }
                          ],
                          ""_source"": {
                              ""excludes"": []
                          },
                          ""stored_fields"": [
                            ""*""
                          ],
                          ""from"": " + ((logsPerformanceRequest.PageIndex - 1) * logsPerformanceRequest.PageSize) + @","
                                                + @"""size"": " + logsPerformanceRequest.PageSize + @"," +
                          @"""query"": {
                            ""bool"": {
                              ""filter"": ["
                                  + strQueryConditions +
                                  (logsPerformanceRequest.FromDate.HasValue || logsPerformanceRequest.ToDate.HasValue ?
                                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) + @"{
                                    ""range"": {
                                      ""@timestamp"": {" +
                                            (logsPerformanceRequest.FromDate.HasValue && logsPerformanceRequest.FromDate.Value > DateTime.MinValue ? @"""gte"": """ + logsPerformanceRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty) +
                                            (logsPerformanceRequest.ToDate.HasValue && logsPerformanceRequest.ToDate.Value > DateTime.MinValue ? @"""lte"": """ + logsPerformanceRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty) +
                                            @"""format"": ""strict_date_optional_time""
                                      }
                                    }
                                  }" : string.Empty)
                              + @"]
                            }
                          }
                        }";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body1, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        /// <param name="logSecurityRequest">configs.</param>
        /// <param name="sort">sort.</param>
        public async Task<string> GetLogSecurity(LogSecurityRequestDto logSecurityRequest, string sort = "desc")
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (logSecurityRequest.Hosts.Count > 0)
            {
                var host = @"{
                     ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterWildcard("rule.description", new List<string> { logSecurityRequest.EventName })
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    },
                    {
                     ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterWildcard("agent.name", new List<string> { logSecurityRequest.HostName })
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    },
                    {
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", logSecurityRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }
                    ";

                lstQueryCondition.Add(host);
            }

            if (logSecurityRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { logSecurityRequest.Level.Value.ToString() }));
            }

            if (!string.IsNullOrEmpty(logSecurityRequest.EventGroup))
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.groups", new List<string> { logSecurityRequest.EventGroup }));
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
  ""sort"": [
    {
      ""@timestamp"": {
        ""order"": """ + logSecurityRequest.Sort + @""",
        ""unmapped_type"": ""boolean""
      }
    }
  ],
""size"": 500,
   ""_source"":{
      ""includes"":[
         ""agent.name"",
         ""rule.description"",
         ""rule.level"",
         ""rule.groups"",
         ""rule.mitre.technique"",
         ""full_log"",
         ""data.vulnerability.title"",
         ""data.vulnerability.rationale"",
         ""data.vulnerability.references"",
         ""data.integration"",
         ""data.virustotal.description"",
         ""rule.mitre.technique"",
         ""rule.pci_dss"",
         ""syscheck.path"",
         ""@timestamp""
      ],
      ""excludes"":[]
   },
    ""track_total_hits"": true,
   ""query"":{
      ""bool"":{
        ""must"":[{
            ""exists"": {
            ""field"": ""rule""
          }}],
         ""filter"":[" +
                  strQueryConditions
                + (logSecurityRequest.FromDate.HasValue || logSecurityRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (logSecurityRequest.FromDate.HasValue ? @"""gte"":""" + logSecurityRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (logSecurityRequest.ToDate.HasValue ? @"""lte"":""" + logSecurityRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"]
      }
    }
}";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        /// <param name="logSecurityRequest">configs.</param>
        /// <param name="sort">sort.</param>
        public async Task<string> GetLogSecurityMoreThanLevel9(LogSecurityRequestDto logSecurityRequest, string sort = "")
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (logSecurityRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", logSecurityRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
  ""sort"": [
    {" + (string.IsNullOrEmpty(sort) ?
        string.Empty :
        @"""" + sort + @""": {
          ""order"": ""desc"",
          ""unmapped_type"": ""boolean""
        },")
        +
        @"""@timestamp"": {
          ""order"": ""desc"",
          ""unmapped_type"": ""boolean""
        }
      }
    ],
  ""from"": """ + ((logSecurityRequest.PageIndex - 1) * logSecurityRequest.PageSize) + @""",
  ""size"": """ + logSecurityRequest.PageSize + @""",
  ""track_total_hits"": true,
  ""query"":{
    ""bool"":{
      ""must"":[{
          ""exists"": {
          ""field"": ""rule""
        }}],
        ""filter"":[
          {
            ""bool"": {
              ""should"": [
                {
                  ""match_phrase"": {
                    ""rule.level"" : ""10""
                  }
                },
                {
                  ""match_phrase"": {
                    ""rule.level"" : ""11""
                  }
                },
                {
                  ""match_phrase"": {
                    ""rule.level"" : ""12""
                  }
                },
                {
                  ""match_phrase"": {
                    ""rule.level"" : ""13""
                  }
                },
                {
                  ""match_phrase"": {
                    ""rule.level"" : ""14""
                  }
                },
                {
                  ""match_phrase"": {
                    ""rule.level"" : ""15""
                  }
                }
              ],
              ""minimum_should_match"": 1
            }
          }," +
                strQueryConditions
              + (logSecurityRequest.FromDate.HasValue || logSecurityRequest.ToDate.HasValue ?
                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                  @"{
              ""range"":{
                ""timestamp"":{"
                  + (logSecurityRequest.FromDate.HasValue ? @"""gte"":""" + logSecurityRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                  + (logSecurityRequest.ToDate.HasValue ? @"""lte"":""" + logSecurityRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                  + @"""format"":""strict_date_optional_time""
                }
              }
          }" : string.Empty)
            + @"]
    }
  }
}";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetDashboardSummary(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                          this.BuildFilterCondition("name", dataRequest.Hosts)
                        + @"],
                      ""minimum_should_match"": 1
                    }
                  }";
                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-monitoring-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
                    ""size"": 0,
                    ""aggs"": {
                      ""datas"": {
                        ""terms"": {
                          ""field"": ""status""
                        }
                      }
                    },
                    ""_source"": {
                      ""excludes"": []
                    },
                    ""track_total_hits"": true,
                    ""query"": {
                      ""bool"": {
                        ""filter"":[" +
                                strQueryConditions
                                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                @"{
                            ""range"":{
                              ""timestamp"":{"
                                      + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                      + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                      + @"""format"":""strict_date_optional_time""
                              }
                            }
                          }" : string.Empty)
                              + @"]
                      }
                    }
                  }";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetHostStatistic(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();
            if (dataRequest.Hosts?.Count > 0)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("agent.name", dataRequest.Hosts));
            }

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                }
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";

            using var client = new HttpClient();
            var body = @"{
  ""_source"": {
                ""includes"": [
                  ""agent.name"",
                  ""rule.description""
    ],
    ""excludes"": [
      ""@timestamp""
    ]
    },
  ""query"": {
    ""bool"": {
      ""filter"": [
        " +
                    (dataRequest.Hosts?.Count > 0 ?
                           @"{
          ""bool"": {
            ""should"": [" +
                           this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          
        }" : string.Empty)
      + @"
          
        },
        {
          ""range"": {
            ""timestamp"": {
              ""gte"": """ + dataRequest.FromDate.Value.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate.Value.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ]
    }
  }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GeTotaltSecurityEventBetweenLevel(HostStatisticRequestDto dataRequest, int from = 10, int to = 15)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                          this.BuildFilterMonitor("agent.name", dataRequest.Hosts)
                        + @"],
                      ""minimum_should_match"": 1
                    }
                  }";
                lstQueryCondition.Add(host);
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
                    ""size"": 0,
                    ""_source"": {
                      ""excludes"": []
                    },
                    ""stored_fields"": [
                      ""*""
                    ],
                    ""track_total_hits"": true,
                    ""query"": {
                      ""bool"": {
                        ""filter"":[" +
                                  strQueryConditions +
                                  (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ? (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"
                            {
                              ""range"": {
                                ""rule.level"": {
                                  ""gte"": " + from + @",
                                  ""lt"": " + to + @"
                                }
                              }
                            },
                            {
                            ""range"":{
                              ""timestamp"":{"
                                + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                + @"""format"":""strict_date_optional_time""
                              }
                            }
                          }" : string.Empty)
                        + @"]
                      }
                    }
                  }";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetSecurityEventByTime(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                          this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                        + @"],
                      ""minimum_should_match"": 1
                    }
                  }";
                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
                            ""aggs"": {
                              ""datas"": {
                                ""date_histogram"": {
                                  ""field"": ""timestamp"",
                                  ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
                                  ""time_zone"": ""Asia/Saigon"",
                                  ""min_doc_count"": 0
                                }
                              }
                            },
                            ""size"": 0,
                            ""_source"": {
                              ""excludes"": []
                            },
                            ""stored_fields"": [
                              ""*""
                            ],
                            ""query"": {
                              ""bool"": {
                                ""filter"":[" +
                                  strQueryConditions +
                                  (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ? (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"{
                                    ""range"":{
                                      ""timestamp"":{"
                                        + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                        + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                        + @"""format"":""strict_date_optional_time""
                                      }
                                    }
                                  }" : string.Empty)
                                + @"]
                              }
                            }
                          }";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetTopEventsByLevel(LogSecurityRequestDto dataRequest)
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                                        this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                        + @"
                      ],
                      ""minimum_should_match"": 1
                    }
                  }";

                lstQueryCondition.Add(host);
            }

            switch (dataRequest.Type)
            {
                case SecurityTabs.MITREATTCK:
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""cluster.name"": ""wazuh""
                      }
                    }");
                    lstQueryCondition.Add(@"{
                      ""exists"": {
                        ""field"": ""rule.mitre.id""
                      }
                    }");
                    break;
                case SecurityTabs.PCIDSS:
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""cluster.name"": ""wazuh""
                      }
                    }");
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""rule.groups"": ""vulnerability-detector""
                      }
                    }");
                    break;
                case SecurityTabs.SecurityEvents:
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""cluster.name"": ""wazuh""
                      }
                    }");
                    break;
                case SecurityTabs.SecurityIntegrityMonitoring:
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""cluster.name"": ""wazuh""
                      }
                    }");
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""rule.groups"": ""syscheck""
                      }
                    }");
                    break;
                case SecurityTabs.SecurityVulnerabilities:
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""cluster.name"": ""wazuh""
                      }
                    }");
                    lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""rule.groups"": ""vulnerability-detector""
                      }
                    }");
                    break;
                default:
                    break;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
                            ""size"": ""0"",
                            ""aggs"": {
                              ""datas"": {
                                ""terms"": {
                                  ""field"": ""rule.description"",
                                  ""order"": {
                                    ""level"": ""desc""
                                  },
                                  ""size"": 10
                                },
                                ""aggs"": {
                                  ""level"": {
                                    ""max"": {
                                      ""field"": ""rule.level""
                                    }
                                  },
                                ""hosts"": {
                                          ""terms"": {
                                                ""field"": ""agent.name""
                                          }
                                        }
                                    }
                                }
                            },
                            ""_source"": {
                              ""excludes"": []
                            },
                            ""query"": {
                              ""bool"": {
                                ""must"": [
                                  {
                                    ""exists"": {
                                      ""field"": ""rule""
                                    }
                                  }
                                ],
                                ""filter"": [" +
                                  strQueryConditions
                                  + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"{
                                    ""range"":{
                                        ""timestamp"":{"
                                          + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + @"""format"":""strict_date_optional_time""
                                        }
                                    }
                                  }" : string.Empty)
                                + @"]
                              }
                            }
                          }";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetGroupLogsByCondition(LogSecurityRequestDto dataRequest)
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                     ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterWildcard("rule.description", new List<string> { dataRequest.EventName })
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    },
                    {
                     ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterWildcard("agent.name", new List<string> { dataRequest.HostName })
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    },
                    {
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }
                    ";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            lstQueryCondition.Add(@"{
                      ""match_phrase"": {
                        ""cluster.name"": ""wazuh""
                      }
                    }");

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
                            ""size"": ""0"",
                            ""aggs"": {
                              ""datas"": {
                                ""terms"": {
                                  ""field"": ""rule.description"",
                                  ""order"": {
                                    ""level"": ""desc""
                                  },
                                   ""size"": ""500""
                                },
                                ""aggs"": {
                                  ""level"": {
                                    ""max"": {
                                      ""field"": ""rule.level""
                                    }
                                  },
                                ""hosts"": {
                                          ""terms"": {
                                                ""field"": ""agent.name""
                                          }
                                        }
                                    }
                                }
                            },
                            ""_source"": {
                              ""excludes"": []
                            },
                            ""query"": {
                              ""bool"": {
                                ""must"": [
                                  {
                                    ""exists"": {
                                      ""field"": ""rule""
                                    }
                                  }
                                ],
                                ""filter"": [" +
                                  strQueryConditions
                                  + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"{
                                    ""range"":{
                                        ""timestamp"":{"
                                          + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + @"""format"":""strict_date_optional_time""
                                        }
                                    }
                                  }" : string.Empty)
                                + @"]
                              }
                            }
                          }";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetRuleGroupByAgentName(LogSecurityRequestDto dataRequest)
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            var body = @"{
                            ""size"": ""0"",
                            ""query"": {
                              ""bool"": {
                                ""filter"": [
                                    {
                                    ""range"":{
                                        ""timestamp"":{"
                                          + (dataRequest.FromDate.HasValue ?
                                          @"""gte"":""" + dataRequest.FromDate.Value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + @"""," : string.Empty)
                                          + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + @"""," : string.Empty)
                                          + @"""format"":""epoch_millis""
                                        }
                                      }
                                    },
                                    {
                                      ""query_string"": {
                                                ""analyze_wildcard"": true,
                                                ""query"": ""agent.name: " + dataRequest.HostName + @" ""
                                      }
                                    }
                                ]
                              }
                            },
                            ""aggs"": {
                                ""2"": {
                                            ""terms"": {
                                                ""field"": ""rule.groups"",
                                    ""size"": 10,
                                    ""order"": {
                                                    ""_count"": ""desc""
                                    },
                                    ""min_doc_count"": 1
                                            },
                                  ""aggs"": { }
                                        }
                                    }
            }";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetHostsInEvents(LogSecurityRequestDto dataRequest)
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                                        this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                        + @"
                      ],
                      ""minimum_should_match"": 1
                    }
                  }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.EventList.Count > 0)
            {
                var events = @"{
                    ""bool"": {
                      ""should"": [" +
                                        this.BuildFilterCondition("rule.description", dataRequest.EventList)
                                        + @"
                      ],
                      ""minimum_should_match"": 1
                    }
                  }";

                lstQueryCondition.Add(events);
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
                            ""_source"": {
                              ""excludes"": []
                            },
                            ""size"": 1000,
                            ""query"": {
                              ""bool"": {
                                ""must"": [
                                ],
                                ""filter"": [" +
                                  strQueryConditions
                                  + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"{
                                    ""range"":{
                                        ""timestamp"":{"
                                          + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + @"""format"":""strict_date_optional_time""
                                        }
                                    }
                                  }" : string.Empty)
                                + @"]
                              }
                            }
                          }";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage response = new HttpResponseMessage();

            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetAlertsEvolutionOverTime(LogSecurityRequestDto dataRequest)
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                                        this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                        + @"
                      ],
                      ""minimum_should_match"": 1
                    }
                  }";
                lstQueryCondition.Add(host);
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
                            ""size"": ""0"",
                            ""aggs"": {
                              ""datas"": {
                                ""terms"": {
                                  ""field"": ""rule.mitre.technique""
                                },
                                ""aggs"": {
                                  ""historys"": {
                                    ""date_histogram"": {
                                      ""field"": ""timestamp"",
                                      ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
                                      ""time_zone"": ""Asia/Saigon"",
                                      ""min_doc_count"": 1
                                    }
                                  }
                                }
                              }
                            },                 
                            ""_source"": {
                              ""excludes"": []
                            },
                            ""stored_fields"": [
                              ""*""
                            ],
                            ""query"": {
                              ""bool"": {
                                ""must"": [
                                  {
                                    ""exists"": {
                                      ""field"": ""rule""
                                    }
                                  }
                                ],
                                ""filter"": [                        
                                  {
                                    ""match_phrase"": {
                                      ""cluster.name"": {
                                        ""query"": ""wazuh""
                                      }
                                    }
                                  },
                                  {
                                    ""exists"": {
                                      ""field"": ""rule.mitre.id""
                                    }
                                  },
                                  " +
                                  strQueryConditions
                                  + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"{
                                    ""range"":{
                                        ""timestamp"":{"
                                          + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + @"""format"":""strict_date_optional_time""
                                        }
                                    }
                                  }" : string.Empty)
                                + @"]
                              }
                            }
                          }";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetAgentsStatus(LogSecurityRequestDto dataRequest)
        {
            var url = $"{this.configs["ElasticSeachUrl"]}/wazuh-monitoring-3.x-*/_search";
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                                        this.BuildFilterMonitor("name", dataRequest.Hosts)
                                        + @"
                      ],
                      ""minimum_should_match"": 1
                    }
                  }";
                lstQueryCondition.Add(host);
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var body = @"{
                            ""size"": ""0"",
                            ""aggs"": {
                              ""datas"": {
                                ""terms"": {
                                  ""field"": ""status""
                                },
                                ""aggs"": {
                                  ""historys"": {
                                    ""date_histogram"": {
                                      ""field"": ""timestamp"",
                                      ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
                                      ""time_zone"": ""Asia/Saigon"",
                                      ""min_doc_count"": 1
                                    }
                                  }
                                }
                              }
                            },
                            ""_source"": {
                                ""excludes"": []
                            },
                            ""stored_fields"": [
                              ""*""
                            ],
                            ""query"": {
                              ""bool"": {
                                ""filter"": [
                                  " +
                                  strQueryConditions
                                  + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                  (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                  @"{
                                    ""range"":{
                                        ""timestamp"":{"
                                          + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                          + @"""format"":""strict_date_optional_time""
                                        }
                                    }
                                  }" : string.Empty)
                                + @"]
                              }
                            }
                          }";
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetMostCommonCVEs(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
        ""2"": {
          ""terms"": {
            ""field"": ""data.vulnerability.cve"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
          }
        }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
                {
                ""match_phrase"":{
                ""cluster.name"":{
                ""query"":""wazuh""
                }
                }
                },
                {
                    ""match_phrase"":{
                        ""rule.groups"":{
                            ""query"":""vulnerability-detector""
                        }
                    }
                },
                " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"]
    }
  }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetVulnerabilitiesSummary(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
    ""2"": {
      ""terms"": {
            ""field"": ""agent.name"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
      },
      ""aggs"": {
        ""3"": {
          ""terms"": {
            ""field"": ""data.vulnerability.severity"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
          }
        }
      }
    }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
                {
                  ""match_phrase"":{
                    ""cluster.name"":{
                      ""query"":""wazuh""
                    }
                  }
                },
                {
                    ""match_phrase"":{
                        ""rule.groups"":{
                            ""query"":""vulnerability-detector""
                        }
                    }
                },
                " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"]
    }
  }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetMostCommonCWEs(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
        ""2"": {
          ""terms"": {
            ""field"": ""data.vulnerability.cwe_reference"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
          }
        }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
                {
                  ""match_phrase"":{
                    ""cluster.name"":{
                      ""query"":""wazuh""
                    }
                  }
                },
                {
                    ""match_phrase"":{
                        ""rule.groups"":{
                            ""query"":""vulnerability-detector""
                        }
                    }
                },
                " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"]
    }
  }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetMostAffectedAgents(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
        ""2"": {
          ""terms"": {
            ""field"": ""agent.name"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
          }
        }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
                {
                  ""match_phrase"":{
                    ""cluster.name"":{
                      ""query"":""wazuh""
                    }
                  }
                },
                {
                    ""match_phrase"":{
                        ""rule.groups"":{
                            ""query"":""vulnerability-detector""
                        }
                    }
                },
                " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"]
    }
  }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetAlertsSeverity(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterMonitor("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
    ""datas"": {
      ""terms"": {
        ""field"": ""data.vulnerability.severity""
      },
      ""aggs"": {
        ""historys"": {
          ""date_histogram"": {
            ""field"": ""timestamp"",
            ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
            ""time_zone"": ""Asia/Saigon"",
            ""min_doc_count"": 1
          }
        }
      }
    }
  },  
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
              {
                ""match_phrase"":{
                  ""cluster.name"":{
                    ""query"":""wazuh""
                  }
                }
              },
              {
                ""match_phrase"":{
                  ""rule.groups"":{
                      ""query"":""vulnerability-detector""
                  }
                }
              },
                " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
      + @"]
    }
  }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetAlertsByActionOverTime(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterMonitor("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
                    ""aggs"": {
                      ""datas"": {
                        ""terms"": {
                          ""field"": ""syscheck.event""
                        },
                        ""aggs"": {
                          ""historys"": {
                            ""date_histogram"": {
                              ""field"": ""timestamp"",
                              ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
                              ""time_zone"": ""Asia/Saigon"",
                              ""min_doc_count"": 1
                            }
                          }
                        }
                      }
                    },                 
                    ""_source"": {
                      ""excludes"": []
                    },
                    ""stored_fields"": [
                      ""*""
                    ],
                    ""query"": {
                      ""bool"": {
                        ""filter"":[
                          {
                            ""match_phrase"":{
                              ""cluster.name"":{
                                ""query"":""wazuh""
                              }
                            }
                          },
                          {
                            ""match_phrase"":{
                              ""rule.groups"":{
                                ""query"":""syscheck""
                              }
                            }
                          }," +
                                strQueryConditions
                                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                @"{
                            ""range"":{
                              ""timestamp"":{"
                                      + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                      + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                      + @"""format"":""strict_date_optional_time""
                              }
                            }
                          }" : string.Empty)
                              + @"]
                      }
                    }
                  }";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetTop5SecurityEvent(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
        ""2"": {
          ""terms"": {
            ""field"": ""agent.name"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
        }
     }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
            {
            ""match_all"":{}
            },
            {
            ""match_phrase"":{
                ""cluster.name"":{
                    ""query"":""wazuh""
                    }
                }
            },
            " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""@timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"
        ]
        }
      }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetTop5AgentIntegrityMonitoring(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
        ""2"": {
          ""terms"": {
            ""field"": ""agent.name"",
            ""order"": {
              ""_count"": ""desc""
            },
            ""size"": 5
        }
     }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
            {
            ""match_all"":{}
            },
            {
            ""match_phrase"":{
                ""cluster.name"":{
                    ""query"":""wazuh""
                    }
                }
            },
            {
            ""match_phrase"":{
                ""rule.groups"":{
                    ""query"":""syscheck""
                    }
                }
            },
            " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""@timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"
        ],
          ""should"": [],
          ""must_not"": []
        }
      }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetEventSummaryIntegrityMonitoring(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                      ""bool"": {
                        ""should"": [" +
                                       this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                                       + @"
                        ],
                        ""minimum_should_match"": 1
                      }
                    }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Level.HasValue)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("rule.level", new List<string> { dataRequest.Level.Value.ToString() }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
        ""2"": {
            ""date_histogram"":{
            ""field"":""timestamp"",
            ""fixed_interval"":""" + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
            ""time_zone"":""Asia/Saigon"",
            ""min_doc_count"":1
            }
        }
    },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"":[
            {
            ""match_all"":{}
            },
            {
            ""match_phrase"":{
                ""cluster.name"":{
                    ""query"":""wazuh""
                    }
                }
            },
            {
            ""match_phrase"":{
                ""rule.groups"":{
                    ""query"":""syscheck""
                    }
                }
            },
            " +
                  strQueryConditions
                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                    (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                    @"{
               ""range"":{
                  ""@timestamp"":{"
                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                    + @"""format"":""strict_date_optional_time""
                  }
               }
            }" : string.Empty)
        + @"
        ],
          ""should"": [],
          ""must_not"": []
        }
      }
}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetPerformanceEvent(HostStatisticRequestDto dataRequest)
        {
            var lstQueryCondition = new List<string>();

            if (dataRequest.Hosts.Count > 0)
            {
                var host = @"{
                    ""bool"": {
                      ""should"": [" +
                                  this.BuildFilterMonitor("event_name1", dataRequest.Hosts)
                                  + @"
                        ],
                      ""minimum_should_match"": 1
                    }
                  }";

                lstQueryCondition.Add(host);
            }

            if (dataRequest.Serverity != null)
            {
                lstQueryCondition.Add(this.BuildFilterCondition("trigger_severity", new List<string> { dataRequest.Serverity }));
            }

            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var strQueryConditions = string.Empty;

            for (var i = 0; i < lstQueryCondition.Count; i++)
            {
                if (i > 0)
                {
                    strQueryConditions += ",";
                }

                strQueryConditions += lstQueryCondition[i];
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/zabbix-problem-2-*/_search/";
            var client = new HttpClient();
            var body = @"{
                    ""aggs"": {
                      ""datas"": {
                        ""date_histogram"": {
                          ""field"": ""@timestamp"",
                          ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
                          ""time_zone"": ""Asia/Saigon"",
                          ""min_doc_count"": 0
                        }
                      }
                    },
                    ""size"": 0,          
                    ""_source"": {
                      ""excludes"": []
                    },
                    ""stored_fields"": [
                      ""*""
                    ],
                    ""query"": {
                      ""bool"": {
                        ""filter"":[" +
                                strQueryConditions
                                + (dataRequest.FromDate.HasValue || dataRequest.ToDate.HasValue ?
                                (!string.IsNullOrEmpty(strQueryConditions) ? "," : string.Empty) +
                                @"{
                            ""range"":{
                              ""@timestamp"":{"
                                    + (dataRequest.FromDate.HasValue ? @"""gte"":""" + dataRequest.FromDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                    + (dataRequest.ToDate.HasValue ? @"""lte"":""" + dataRequest.ToDate.Value.ToUniversalTime().ToString("o") + @"""," : string.Empty)
                                    + @"""format"":""strict_date_optional_time""
                              }
                            }
                          }" : string.Empty)
                              + @"]
                      }
                    }
                  }";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetTop10SecurityEvent(HostStatisticRequestDto dataRequest)
        {
            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
    ""2"": {
      ""terms"": {
        ""field"": ""rule.description"",
        ""order"": {
          ""_count"": ""desc""
        },
        ""size"": 10
      }
    }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"": [" +
                       (dataRequest.Hosts?.Count > 0 ?
                           @"{
          ""bool"": {
            ""should"": [" +
                           this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          }
        }," : string.Empty)
                       + @"
        {
          ""range"": {
            ""timestamp"": {
             ""gte"": """ + dataRequest.FromDate.Value.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate.Value.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ]
    }
  }
}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetTop10AttackIP(HostStatisticRequestDto dataRequest, int ruleId)
        {
            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
  ""aggs"": {
    ""2"": {
      ""terms"": {
        ""field"": ""data.srcip"",
        ""order"": {
          ""_count"": ""desc""
        },
        ""size"": 10
      }
    }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"": [" +
                       (dataRequest.Hosts?.Count > 0 ?
                           @"{
          ""bool"": {
            ""should"": [" +
                           this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          }
        }," : string.Empty)
                       + @"
        {
          ""bool"": {
            ""filter"": [
              {
                ""bool"": {
                  ""should"": [
                    {
                      ""match"": {
                        ""rule.id"": " + ruleId + @"
                      }
                    }
                  ],
                  ""minimum_should_match"": 1
                }
              }
            ]
          }
        },
        {
          ""range"": {
            ""timestamp"": {
              ""gte"": """ + dataRequest.FromDate.Value.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate.Value.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ]
    }
  }
}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetLast10SecurityEvent(HostStatisticRequestDto dataRequest)
        {
            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            var client = new HttpClient();
            var body = @"{
""sort"": [
    {
      ""@timestamp"": {
        ""order"": ""desc"",
        ""unmapped_type"": ""boolean""
      }
    }
  ],
  ""_source"": {
    ""includes"": [
      ""@timestamp"",
      ""agent.name"",
      ""rule.description""
    ]
  },
  ""query"": {
    ""bool"": {
      ""must"":[{
            ""exists"": {
            ""field"": ""rule""
          }}],
      ""filter"": [
" +
                       (dataRequest.Hosts?.Count > 0 ?
                           @"{
          ""bool"": {
            ""should"": [" +
                           this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          }
        }," : string.Empty)
                       + @"
        {
          ""bool"": {
            ""filter"": [
            ]
          }
        },
        {
          ""range"": {
            ""@timestamp"": {
              ""gte"": """ + dataRequest.FromDate.Value.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate.Value.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ]
    }
  }
}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetLast10PerformanceEvent(HostStatisticRequestDto dataRequest)
        {
            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/zabbix-problem-2-*/_search/";
            var client = new HttpClient();
            var body = @"{
""sort"": [
    {
      ""@timestamp"": {
        ""order"": ""desc"",
        ""unmapped_type"": ""boolean""
      }
    }
  ],
  ""_source"": {
    ""includes"": [
      ""@timestamp"",
      ""event_host1"",
      ""trigger_name""
    ]
  },
  ""query"": {
    ""bool"": {
      ""filter"": [
" +
                       (dataRequest.Hosts?.Count > 0 ?
                           @"{
          ""bool"": {
            ""should"": [" +
                           this.BuildFilterCondition("event_host1", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          }
        }," : string.Empty)
                       + @"
        {
          ""bool"": {
            ""filter"": [
              {
                ""bool"": {
                  ""should"": [
                    {
                      ""match"": {
                        ""trigger_status"": ""PROBLEM""
                      }
                    }
                  ],
                  ""minimum_should_match"": 1
                }
              }
            ]
          }
        },
        {
          ""range"": {
            ""@timestamp"": {
              ""gte"": """ + dataRequest.FromDate?.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate?.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ]
    }
  }
}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetTop10AgentsByAlertsNumber(HostStatisticRequestDto dataRequest)
        {
            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            using var client = new HttpClient();

            string body = @"{
  ""aggs"": {
    ""2"": {
      ""terms"": {
        ""field"": ""agent.name"",
        ""order"": {
          ""_count"": ""desc""
        },
        ""size"": 10
      }
    }
  },
  ""size"": 0,
  ""_source"": {
    ""excludes"": []
  },
  ""stored_fields"": [
    ""*""
  ],
  ""query"": {
    ""bool"": {
      ""filter"": [ " +
        (dataRequest.Hosts?.Count > 0 ?
                           @"{
                ""bool"": {
                    ""should"": [" +
                           this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          }
        }," : string.Empty) + @"
        {
          ""match_all"": {}
        },
        {
          ""match_phrase"": {
            ""cluster.name"": {
              ""query"": ""wazuh""
            }
          }
        },
        {
          ""exists"": {
            ""field"": ""rule.pci_dss""
          }
        },
        {
          ""range"": {
            ""@timestamp"": {
              ""gte"": """ + dataRequest.FromDate.Value.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate.Value.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ],
      ""should"": [],
      ""must_not"": []
    }
  }
}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        /// <inheritdoc/>
        public async Task<string> GetPCIDSSRequirements(HostStatisticRequestDto dataRequest)
        {
            if (!dataRequest.FromDate.HasValue)
            {
                dataRequest.FromDate = DateTime.MinValue;
            }

            if (!dataRequest.ToDate.HasValue)
            {
                dataRequest.ToDate = DateTime.Now;
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/wazuh-alerts-3.x-*/_search/";
            using var client = new HttpClient();

            var body = @"{
  ""aggs"": {
    ""2"": {
      ""date_histogram"": {
        ""field"": ""timestamp"",
        ""fixed_interval"": """ + CommonUtilities.GetCalendarInterval(dataRequest.FromDate, dataRequest.ToDate) + @""",
        ""time_zone"": ""Asia/Saigon"",
        ""min_doc_count"": 1
      },
      ""aggs"": {
        ""3"": {
          ""terms"": {
            ""field"": ""rule.pci_dss"",
            ""order"": {
              ""_count"": ""desc""
            }
          }
        }
      }
    }
  },
  ""_source"": {
    ""excludes"": []
  },
  ""size"": 0,
  ""query"": {
    ""bool"": {
      ""filter"": [" +
                (dataRequest.Hosts?.Count > 0 ?
                           @"{
                ""bool"": {
                    ""should"": [" +
                           this.BuildFilterCondition("agent.name", dataRequest.Hosts)
                           + @"
            ],
            ""minimum_should_match"": 1
          }
        }," : string.Empty) + @"
        {
          ""match_phrase"": {
            ""cluster.name"": {
              ""query"": ""wazuh""
            }
          }
        },
        {
          ""exists"": {
            ""field"": ""rule.pci_dss""
          }
        },
        {
          ""range"": {
            ""@timestamp"": {
              ""gte"": """ + dataRequest.FromDate.Value.ToString("o") + @""",
              ""lte"": """ + dataRequest.ToDate.Value.ToString("o") + @""",
              ""format"": ""strict_date_optional_time""
            }
          }
        }
      ]
    }
  }
}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private string BuildFilterCondition(string fieldName, List<string> value)
        {
            if (string.IsNullOrEmpty(fieldName) || value == null || value.Count <= 0)
            {
                return string.Empty;
            }

            var result = string.Empty;
            for (var i = 0; i < value.Count; i++)
            {
                if (i > 0)
                {
                    result += ",";
                }

                result += "{\"bool\": { \"should\": [{";

                result += @"""match"": {
                    """ + fieldName + @""": """ + JsonEncodedText.Encode(value[i]) + @"""
                }";
                result += "}],\"minimum_should_match\": 1}}";
            }

            return result;
        }

        private string BuildFilterMonitor(string fieldName, List<string> value)
        {
            if (string.IsNullOrEmpty(fieldName) || value == null || value.Count <= 0)
            {
                return string.Empty;
            }

            var result = string.Empty;
            for (var i = 0; i < value.Count; i++)
            {
                if (i > 0)
                {
                    result += ",";
                }

                result += @"{""match_phrase"": {
            """ + fieldName + @""": """ + value[i] + @"""
        }}";
            }

            return result;
        }

        private string BuildFilterWildcard(string fieldName, List<string> value)
        {
            if (string.IsNullOrEmpty(fieldName) || value == null || value.Count <= 0)
            {
                return string.Empty;
            }

            var result = string.Empty;
            for (var i = 0; i < value.Count; i++)
            {
                if (i > 0)
                {
                    result += ",";
                }

                result += @"{""wildcard"": {
            """ + fieldName + @""": """ + "*" + value[i] + "*" + @"""
        }}";
            }

            return result;
        }

        private async Task<bool> AddNewRecord(string content, Guid id, string index)
        {
            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/{index}/_create/{id}";
            using var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return true;
        }
    }
}
