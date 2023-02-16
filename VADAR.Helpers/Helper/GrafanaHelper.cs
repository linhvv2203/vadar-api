// <copyright file="GrafanaHelper.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers.Enums;
using VADAR.Helpers.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace VADAR.Helpers.Helper
{
    /// <summary>
    /// Grafana Helper Class.
    /// </summary>
    public class GrafanaHelper : IGrafanaHelper
    {
        private readonly IConfiguration configs;
        private readonly string baseUrl;

        /// <summary>
        /// Initialises a new instance of the <see cref="GrafanaHelper"/> class.
        /// </summary>
        /// <param name="configs">configs.</param>
        public GrafanaHelper(IConfiguration configs)
        {
            this.configs = configs;
            this.baseUrl = this.configs["Grafana:BaseUrl"];
        }

        /// <inheritdoc/>
        public async Task<GrafanaFolderDto> CreateNewFolder(string folderTitle)
        {
            if (string.IsNullOrEmpty(folderTitle))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var url = $"{this.baseUrl}/api/folders";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, "{\"title\": \"" + folderTitle + "\"}");
            var response = await this.SendRequest(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GrafanaFolderDto>(responseBody);

            if (result != null && !string.IsNullOrEmpty(result.Uid))
            {
                await this.SetDefaultFolderPermission(result.Uid);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task SetDefaultFolderPermission(string folderUid, IEnumerable<GrafanaPermissionDto> permissions = null)
        {
            if (string.IsNullOrEmpty(folderUid))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var url = $"{this.baseUrl}/api/folders/" + folderUid + "/permissions";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, (permissions == null || !permissions.Any()) ? "{\"items\":[]}" : "{\"items\":" + JsonConvert.SerializeObject(permissions) + "}");
            var response = await this.SendRequest(request);
            await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteFolder(string uId)
        {
            if (string.IsNullOrEmpty(uId))
            {
                return true;
            }

            var url = $"{this.baseUrl}/api/folders/" + uId;

            var request = this.HttpRequestMessage(HttpMethod.Delete, url);
            var response = await this.SendRequest(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <inheritdoc/>
        public async Task<GrafanaDashboardDto> ImportPerformanceDashboard(long folderId, string dashboardTitle, string workspaceGroupName)
        {
            if (folderId <= 0 || string.IsNullOrEmpty(dashboardTitle))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var dashboardTemplate = @"{
   ""dashboard"":{
   ""annotations"":{
      ""list"":[
         {
            ""builtIn"":1,
            ""datasource"":""-- Grafana --"",
            ""enable"":true,
            ""hide"":true,
            ""iconColor"":""rgba(0, 211, 255, 1)"",
            ""name"":""Annotations & Alerts"",
            ""type"":""dashboard""
         }
      ]
   },
   ""editable"":true,
   ""gnetId"":null,
   ""graphTooltip"":0,
   ""iteration"":1600685826936,
   ""links"":[
      
   ],
   ""panels"":[
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":3,
            ""w"":12,
            ""x"":0,
            ""y"":0
         },
         ""id"":38,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""s""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""none"",
            ""justifyMode"":""center"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Status""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""" + workspaceGroupName + @"""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Uptime""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Uptime"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":3,
            ""w"":12,
            ""x"":12,
            ""y"":0
         },
         ""id"":19,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""dateTimeAsIso""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""none"",
            ""justifyMode"":""center"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""General""
               },
               ""functions"":[
                  {
                     ""def"":{
                        ""category"":""Transform"",
                        ""defaultParams"":[
                           100
                        ],
                        ""name"":""scale"",
                        ""params"":[
                           {
                              ""name"":""factor"",
                              ""options"":[
                                 100,
                                 0.01,
                                 10,
                                 -1
                              ],
                              ""type"":""float""
                           }
                        ]
                     },
                     ""params"":[
                        ""1000""
                     ],
                     ""text"":""scale(1000)""
                  }
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""System boot time""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Lần reboot gần nhất"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":6,
            ""x"":0,
            ""y"":3
         },
         ""id"":13,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""mean""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":5
                        },
                        {
                           ""color"":""red"",
                           ""value"":10
                        }
                     ]
                  }
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""CPU""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Load average (5m avg)""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""CPU""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""CPU utilization""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Tải hiện tại"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":6,
            ""x"":6,
            ""y"":3
         },
         ""id"":15,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""#299c46"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""decbytes""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Memory""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Available memory""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Bộ nhớ còn trống"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":3,
            ""w"":4,
            ""x"":12,
            ""y"":3
         },
         ""id"":22,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""last""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":50
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""none"",
            ""justifyMode"":""center"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""General""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Maximum number of processes""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Số lượng tiến trình tối đa"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":3,
            ""w"":5,
            ""x"":16,
            ""y"":3
         },
         ""id"":23,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""last""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":50
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""none"",
            ""justifyMode"":""center"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""General""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Maximum number of open file descriptors""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Số lượng files tối đa có thể mở"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":3,
            ""w"":3,
            ""x"":21,
            ""y"":3
         },
         ""id"":20,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""last""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""none"",
            ""justifyMode"":""center"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""General""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Number of logged in users""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""User đang login"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":6,
            ""x"":12,
            ""y"":6
         },
         ""id"":40,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""#299c46"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""decbytes""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Memory""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Total memory""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Tổng số bộ nhớ"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":6,
            ""x"":18,
            ""y"":6
         },
         ""id"":41,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""#299c46"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""decbytes""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Memory""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Used memory""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Tổng số bộ nhớ đã sử dụng"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":7,
            ""w"":12,
            ""x"":0,
            ""y"":7
         },
         ""id"":17,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""decbits""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Interface eth0""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Interface eth0: Bits received""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Interface eth0""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Interface eth0: Bits sent""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""\bBăng thông cổng eth0"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":12,
            ""x"":12,
            ""y"":10
         },
         ""id"":21,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""last""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""center"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""General""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Number of running processes""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""General""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Number of processes""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Số lượng tiến trình đang chạy"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":8,
            ""w"":12,
            ""x"":0,
            ""y"":14
         },
         ""id"":25,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""reqps""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sda""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sda: Disk read rate""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Disk sda""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sda: Disk write rate""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Đọc ghi ổ cứng /dev/sda"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":7,
            ""w"":12,
            ""x"":12,
            ""y"":14
         },
         ""id"":24,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""decbits""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Interface eth1""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Interface eth1: Bits received""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Interface eth1""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""Interface eth1: Bits sent""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""\bBăng thông cổng eth1"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":3,
            ""x"":12,
            ""y"":21
         },
         ""id"":31,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""max""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":10
                        },
                        {
                           ""color"":""red"",
                           ""value"":30
                        }
                     ]
                  },
                  ""unit"":""percent""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sda""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sda: Disk utilization""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Max Utilization  /dev/sda"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":8,
            ""w"":9,
            ""x"":15,
            ""y"":21
         },
         ""id"":34,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""ms""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sda""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sda: Disk read request avg waiting time (r_await)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Disk sda""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sda: Disk write request avg waiting time (w_await)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Thời gian chờ /dev/sda"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":8,
            ""w"":12,
            ""x"":0,
            ""y"":22
         },
         ""id"":26,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""reqps""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdb""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdb: Disk read rate""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Disk sdb""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdb: Disk write rate""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Đọc ghi ổ cứng /dev/sdb"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":3,
            ""x"":12,
            ""y"":25
         },
         ""id"":28,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""max""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":1
                        },
                        {
                           ""color"":""red"",
                           ""value"":5
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sda""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sda: Disk average queue size (avgqu-sz)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Disk queue  /dev/sda"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":3,
            ""x"":12,
            ""y"":29
         },
         ""id"":29,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""max""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":10
                        },
                        {
                           ""color"":""red"",
                           ""value"":30
                        }
                     ]
                  },
                  ""unit"":""percent""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdb""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdb: Disk utilization""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Max Utilization  /dev/sdb"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":8,
            ""w"":9,
            ""x"":15,
            ""y"":29
         },
         ""id"":35,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""ms""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdb""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdb: Disk read request avg waiting time (r_await)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Disk sdb""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdb: Disk write request avg waiting time (w_await)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Thời gian chờ /dev/sdb"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":6,
            ""x"":0,
            ""y"":30
         },
         ""id"":27,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""bytes""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Filesystem C:""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""C:: Used space""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Filesystem D:""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""D:: Used space""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Filesystem E:""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""E:: Used space""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""C"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Dung lượng ổ cứng đã sử dụng"",
         ""type"":""stat""
      },
      {
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":6,
            ""x"":6,
            ""y"":30
         },
         ""id"":43,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""bytes""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Filesystem C:""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""C:: Total space""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Filesystem D:""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""D:: Total space""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Filesystem E:""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""E:: Total space""
               },
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""proxy"":{
                  ""filter"":""""
               },
               ""queryType"":0,
               ""refId"":""C"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""tags"":{
                  ""filter"":""""
               },
               ""trigger"":{
                  ""filter"":""""
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Tổng số dung lượng của ổ cứng"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":3,
            ""x"":12,
            ""y"":33
         },
         ""id"":32,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""max""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":1
                        },
                        {
                           ""color"":""red"",
                           ""value"":5
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdb""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdb: Disk average queue size (avgqu-sz)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Disk queue  /dev/sdb"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":3,
            ""x"":0,
            ""y"":34
         },
         ""id"":30,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""max""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":10
                        },
                        {
                           ""color"":""red"",
                           ""value"":30
                        }
                     ]
                  },
                  ""unit"":""percent""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdc""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdc: Disk utilization""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Max Utilization  /dev/sdc"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":4,
            ""w"":3,
            ""x"":3,
            ""y"":34
         },
         ""id"":33,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""max""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""#EAB839"",
                           ""value"":1
                        },
                        {
                           ""color"":""red"",
                           ""value"":5
                        }
                     ]
                  },
                  ""unit"":""none""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdc""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdc: Disk average queue size (avgqu-sz)""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Disk queue  /dev/sdb"",
         ""type"":""stat""
      },
      {
         ""cacheTimeout"":null,
         ""datasource"":""Zabbix-Vsec"",
         ""gridPos"":{
            ""h"":8,
            ""w"":12,
            ""x"":12,
            ""y"":37
         },
         ""id"":42,
         ""links"":[
            
         ],
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""lastNotNull""
               ],
               ""defaults"":{
                  ""mappings"":[
                     {
                        ""id"":0,
                        ""op"":""="",
                        ""text"":""N/A"",
                        ""type"":1,
                        ""value"":""null""
                     }
                  ],
                  ""nullValueMode"":""connected"",
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100000000
                        }
                     ]
                  },
                  ""unit"":""reqps""
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""horizontal""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""application"":{
                  ""filter"":""Disk sdc""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdc: Disk read rate""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""A"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            },
            {
               ""application"":{
                  ""filter"":""Disk sdc""
               },
               ""functions"":[
                  
               ],
               ""group"":{
                  ""filter"":""Discovered hosts""
               },
               ""host"":{
                  ""filter"":""$Agent_Name""
               },
               ""item"":{
                  ""filter"":""sdc: Disk write rate""
               },
               ""mode"":0,
               ""options"":{
                  ""showDisabledItems"":false,
                  ""skipEmptyValues"":false
               },
               ""refId"":""B"",
               ""resultFormat"":""time_series"",
               ""table"":{
                  ""skipEmptyValues"":false
               },
               ""triggers"":{
                  ""acknowledged"":2,
                  ""count"":true,
                  ""minSeverity"":3
               }
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Đọc ghi ổ cứng /dev/sdc"",
         ""type"":""stat""
      }
   ],
   ""schemaVersion"":22,
   ""style"":""dark"",
   ""tags"":[
      
   ],
   ""templating"":{
      ""list"":[
         {
            ""allValue"":null,
            ""current"":{
               ""selected"":false
            },
            ""datasource"":""wazuh-monitoring"",
            ""definition"":""{\""find\"": \""terms\"", \""field\"": \""name\"", \""query\"": \""group:" + workspaceGroupName + @"\""}"",
            ""hide"":0,
            ""includeAll"":false,
            ""index"":-1,
            ""label"":null,
            ""multi"":false,
            ""name"":""Agent_Name"",
            ""options"":[
               
            ],
            ""query"":""{\""find\"": \""terms\"", \""field\"": \""name\"", \""query\"": \""group:" + workspaceGroupName + @"\""}"",
            ""refresh"":2,
            ""regex"":"""",
            ""skipUrlSync"":false,
            ""sort"":1,
            ""tagValuesQuery"":"""",
            ""tags"":[
               
            ],
            ""tagsQuery"":"""",
            ""type"":""query"",
            ""useTags"":false
         }
      ]
   },
   ""time"":{
      ""from"":""now-24h"",
      ""to"":""now""
   },
   ""timepicker"":{
      ""refresh_intervals"":[
         ""5s"",
         ""10s"",
         ""30s"",
         ""1m"",
         ""5m"",
         ""15m"",
         ""30m"",
         ""1h"",
         ""2h"",
         ""1d""
      ]
   },
   ""timezone"":"""",
   ""title"":""" + dashboardTitle + @""",
   ""variables"":{
      ""list"":[
         
      ]
   },
   ""version"":2
},
   ""overwrite"":true,
   ""inputs"":[

   ],
   ""folderId"":" + folderId.ToString() + @",
   ""title"":""" + dashboardTitle + @"""
}";
            return await this.ImportNewDashboard(dashboardTemplate);
        }

        /// <inheritdoc/>
        public async Task<GrafanaDashboardDto> ImportInventoryDashboard(long folderId, string dashboardTitle, string workspaceGroupName)
        {
            if (folderId <= 0 || string.IsNullOrEmpty(dashboardTitle))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var dashboardTemplate = @"{
      ""dashboard"":{
  ""annotations"": {
    ""list"": [
      {
        ""builtIn"": 1,
        ""datasource"": ""-- Grafana --"",
        ""enable"": true,
        ""hide"": true,
        ""iconColor"": ""rgba(0, 211, 255, 1)"",
        ""name"": ""Annotations & Alerts"",
        ""type"": ""dashboard""
      }
    ]
  },
  ""editable"": true,
  ""gnetId"": null,
  ""graphTooltip"": 0,
  ""id"": null,
  ""iteration"": 1594029844445,
  ""links"": [],
  ""panels"": [
    {
      ""columns"": [
        {
          ""text"": ""AGENT_NAME"",
          ""value"": ""AGENT_NAME""
        },
        {
          ""text"": ""data.os.name"",
          ""value"": ""data.os.name""
        },
        {
          ""text"": ""data.os.version"",
          ""value"": ""data.os.version""
        },
        {
          ""text"": ""data.release"",
          ""value"": ""data.release""
        }
      ],
      ""datasource"": ""Wazuh Inventory"",
      ""fontSize"": ""100%"",
      ""gridPos"": {
        ""h"": 3,
        ""w"": 9,
        ""x"": 0,
        ""y"": 0
      },
      ""id"": 4,
      ""pageSize"": null,
      ""showHeader"": true,
      ""sort"": {
        ""col"": 0,
        ""desc"": true
      },
      ""styles"": [
        {
          ""alias"": ""Time"",
          ""align"": ""auto"",
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""pattern"": ""Time"",
          ""type"": ""date""
        },
        {
          ""alias"": ""Tên Agent"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""decimals"": 2,
          ""pattern"": ""AGENT_NAME"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Hệ Điều Hành"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.os.name"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Phiên bản"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.os.version"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Phiên bản kernel"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.release"",
          ""sanitize"": false,
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        }
      ],
      ""targets"": [
        {
          ""bucketAggs"": [],
          ""metrics"": [
            {
              ""field"": ""select field"",
              ""id"": ""1"",
              ""meta"": {},
              ""settings"": {
                ""size"": 500
              },
              ""type"": ""raw_document""
            }
          ],
          ""query"": ""AGENT_NAME: \u0022$Agent_Name\u0022 AND inventory_type: os"",
          ""refId"": ""A"",
          ""timeField"": ""@timestamp""
        }
      ],
      ""timeFrom"": null,
      ""timeShift"": null,
      ""title"": ""Thông tin hệ điều hành"",
      ""transform"": ""json"",
      ""type"": ""table""
    },
    {
      ""columns"": [
        {
          ""text"": ""AGENT_NAME"",
          ""value"": ""AGENT_NAME""
        },
        {
          ""text"": ""data.cpu.name"",
          ""value"": ""data.cpu.name""
        },
        {
          ""text"": ""data.cpu.cores"",
          ""value"": ""data.cpu.cores""
        },
        {
          ""text"": ""data.cpu.mhz"",
          ""value"": ""data.cpu.mhz""
        },
        {
          ""text"": ""data.ram.total"",
          ""value"": ""data.ram.total""
        },
        {
          ""text"": ""data.ram.usage"",
          ""value"": ""data.ram.usage""
        }
      ],
      ""datasource"": ""Wazuh Inventory"",
      ""fontSize"": ""100%"",
      ""gridPos"": {
        ""h"": 3,
        ""w"": 15,
        ""x"": 9,
        ""y"": 0
      },
      ""id"": 6,
      ""pageSize"": null,
      ""showHeader"": true,
      ""sort"": {
        ""col"": 0,
        ""desc"": true
      },
      ""styles"": [
        {
          ""alias"": ""Time"",
          ""align"": ""auto"",
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""pattern"": ""Time"",
          ""type"": ""date""
        },
        {
          ""alias"": ""Tên Agent"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""decimals"": 2,
          ""pattern"": ""AGENT_NAME"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Tên CPU"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.cpu.name"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Số lượng cores"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.cpu.cores"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Tốc độ CPU (Mhz)"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 0,
          ""mappingType"": 1,
          ""pattern"": ""data.cpu.mhz"",
          ""sanitize"": false,
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""none""
        },
        {
          ""alias"": ""Dung lượng RAM"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.ram.total"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""deckbytes""
        },
        {
          ""alias"": ""RAM Usage"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.ram.usage"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""percent""
        }
      ],
      ""targets"": [
        {
          ""bucketAggs"": [],
          ""metrics"": [
            {
              ""field"": ""select field"",
              ""id"": ""1"",
              ""meta"": {},
              ""settings"": {
                ""size"": 500
              },
              ""type"": ""raw_document""
            }
          ],
          ""query"": ""AGENT_NAME: \u0022$Agent_Name\u0022 AND inventory_type: hardware"",
          ""refId"": ""A"",
          ""timeField"": ""@timestamp""
        }
      ],
      ""timeFrom"": null,
      ""timeShift"": null,
      ""title"": ""Thông tin hệ điều hành"",
      ""transform"": ""json"",
      ""type"": ""table""
    },
    {
      ""columns"": [
        {
          ""text"": ""AGENT_NAME"",
          ""value"": ""AGENT_NAME""
        },
        {
          ""text"": ""data.items.protocol"",
          ""value"": ""data.items.protocol""
        },
        {
          ""text"": ""data.items.local.ip"",
          ""value"": ""data.items.local.ip""
        },
        {
          ""text"": ""data.items.local.port"",
          ""value"": ""data.items.local.port""
        },
        {
          ""text"": ""data.items.remote.ip"",
          ""value"": ""data.items.remote.ip""
        },
        {
          ""text"": ""data.items.remote.port"",
          ""value"": ""data.items.remote.port""
        },
        {
          ""text"": ""data.items.state"",
          ""value"": ""data.items.state""
        }
      ],
      ""datasource"": ""Wazuh Inventory"",
      ""fontSize"": ""100%"",
      ""gridPos"": {
        ""h"": 10,
        ""w"": 24,
        ""x"": 0,
        ""y"": 3
      },
      ""id"": 8,
      ""pageSize"": null,
      ""showHeader"": true,
      ""sort"": {
        ""col"": 0,
        ""desc"": true
      },
      ""styles"": [
        {
          ""alias"": ""Time"",
          ""align"": ""auto"",
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""pattern"": ""Time"",
          ""type"": ""date""
        },
        {
          ""alias"": ""Tên Agent"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""decimals"": 2,
          ""pattern"": ""AGENT_NAME"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Giao thức"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.protocol"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""IP Local"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.local.ip"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Port Local"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.local.port"",
          ""sanitize"": false,
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""IP Remote"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.remote.ip"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Port Remote"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.remote.port"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Trạng thái"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.state"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""short""
        }
      ],
      ""targets"": [
        {
          ""bucketAggs"": [],
          ""metrics"": [
            {
              ""field"": ""select field"",
              ""id"": ""1"",
              ""meta"": {},
              ""settings"": {
                ""size"": 500
              },
              ""type"": ""raw_document""
            }
          ],
          ""query"": ""AGENT_NAME: \u0022$Agent_Name\u0022 AND inventory_type: ports"",
          ""refId"": ""A"",
          ""timeField"": ""@timestamp""
        }
      ],
      ""timeFrom"": null,
      ""timeShift"": null,
      ""title"": ""Thông tin port kết nối"",
      ""transform"": ""json"",
      ""type"": ""table""
    },
    {
      ""columns"": [
        {
          ""text"": ""AGENT_NAME"",
          ""value"": ""AGENT_NAME""
        },
        {
          ""text"": ""data.items.name"",
          ""value"": ""data.items.name""
        },
        {
          ""text"": ""data.items.cmd"",
          ""value"": ""data.items.cmd""
        }
      ],
      ""datasource"": ""Wazuh Inventory"",
      ""fontSize"": ""100%"",
      ""gridPos"": {
        ""h"": 10,
        ""w"": 24,
        ""x"": 0,
        ""y"": 13
      },
      ""id"": 10,
      ""pageSize"": null,
      ""showHeader"": true,
      ""sort"": {
        ""col"": 0,
        ""desc"": true
      },
      ""styles"": [
        {
          ""alias"": ""Time"",
          ""align"": ""auto"",
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""pattern"": ""Time"",
          ""type"": ""date""
        },
        {
          ""alias"": ""Tên Agent"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""decimals"": 2,
          ""pattern"": ""AGENT_NAME"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Tên tiến trình"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.name"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Command"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.cmd"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""User"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.suser"",
          ""sanitize"": false,
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""CPU Number"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 0,
          ""mappingType"": 1,
          ""pattern"": ""data.items.processor"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Mem resident"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.resident"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""deckbytes""
        },
        {
          ""alias"": ""VM Size"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.vm_size"",
          ""thresholds"": [],
          ""type"": ""number"",
          ""unit"": ""deckbytes""
        }
      ],
      ""targets"": [
        {
          ""bucketAggs"": [],
          ""metrics"": [
            {
              ""field"": ""select field"",
              ""id"": ""1"",
              ""meta"": {},
              ""settings"": {
                ""size"": 500
              },
              ""type"": ""raw_document""
            }
          ],
          ""query"": ""AGENT_NAME: \u0022$Agent_Name\u0022 AND inventory_type: processes"",
          ""refId"": ""A"",
          ""timeField"": ""@timestamp""
        }
      ],
      ""timeFrom"": null,
      ""timeShift"": null,
      ""title"": ""Thông tin tiến trình"",
      ""transform"": ""json"",
      ""type"": ""table""
    },
    {
      ""columns"": [
        {
          ""text"": ""AGENT_NAME"",
          ""value"": ""AGENT_NAME""
        },
        {
          ""text"": ""data.items.name"",
          ""value"": ""data.items.name""
        },
        {
          ""text"": ""data.items.vendor"",
          ""value"": ""data.items.vendor""
        },
        {
          ""text"": ""data.items.install_time"",
          ""value"": ""data.items.install_time""
        },
        {
          ""text"": ""data.items.description"",
          ""value"": ""data.items.description""
        }
      ],
      ""datasource"": ""Wazuh Inventory"",
      ""fontSize"": ""100%"",
      ""gridPos"": {
        ""h"": 10,
        ""w"": 24,
        ""x"": 0,
        ""y"": 23
      },
      ""id"": 12,
      ""pageSize"": null,
      ""showHeader"": true,
      ""sort"": {
        ""col"": 0,
        ""desc"": true
      },
      ""styles"": [
        {
          ""alias"": ""Time"",
          ""align"": ""auto"",
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""pattern"": ""Time"",
          ""type"": ""date""
        },
        {
          ""alias"": ""Tên Agent"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""decimals"": 2,
          ""pattern"": ""AGENT_NAME"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Tên phần mềm"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.name"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Tên nhà cung cấp"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.vendor"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Thời gian cài đặt"",
          ""align"": ""left"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 2,
          ""mappingType"": 1,
          ""pattern"": ""data.items.install_time"",
          ""sanitize"": false,
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        },
        {
          ""alias"": ""Mô tả"",
          ""align"": ""auto"",
          ""colorMode"": null,
          ""colors"": [
            ""rgba(245, 54, 54, 0.9)"",
            ""rgba(237, 129, 40, 0.89)"",
            ""rgba(50, 172, 45, 0.97)""
          ],
          ""dateFormat"": ""YYYY-MM-DD HH:mm:ss"",
          ""decimals"": 0,
          ""mappingType"": 1,
          ""pattern"": ""data.items.description"",
          ""thresholds"": [],
          ""type"": ""string"",
          ""unit"": ""short""
        }
      ],
      ""targets"": [
        {
          ""bucketAggs"": [],
          ""metrics"": [
            {
              ""field"": ""select field"",
              ""id"": ""1"",
              ""meta"": {},
              ""settings"": {
                ""size"": 500
              },
              ""type"": ""raw_document""
            }
          ],
          ""query"": ""AGENT_NAME: \u0022$Agent_Name\u0022 AND inventory_type: packages"",
          ""refId"": ""A"",
          ""timeField"": ""@timestamp""
        }
      ],
      ""timeFrom"": null,
      ""timeShift"": null,
      ""title"": ""Thông tin phần mềm cài đặt"",
      ""transform"": ""json"",
      ""type"": ""table""
    }
  ],
  ""schemaVersion"": 22,
  ""style"": ""dark"",
  ""tags"": [],
  ""templating"": {
    ""list"": [
      {
        ""allValue"": null,
        ""datasource"": ""wazuh-monitoring"",
        ""definition"": ""{\""find\"": \""terms\"", \""field\"": \""name\"", \""query\"": \""group:" + workspaceGroupName + @"\""}"",
        ""hide"": 0,
        ""includeAll"": false,
        ""index"": -1,
        ""label"": null,
        ""multi"": false,
        ""name"": ""Agent_Name"",
        ""options"": [],
        ""query"": ""{\""find\"": \""terms\"", \""field\"": \""name\"", \""query\"": \""group:" + workspaceGroupName + @"\""}"",
        ""refresh"": 1,
        ""regex"": """",
        ""skipUrlSync"": false,
        ""sort"": 0,
        ""tagValuesQuery"": """",
        ""tags"": [],
        ""tagsQuery"": """",
        ""type"": ""query"",
        ""useTags"": false
      }
    ]
  },
  ""time"": {
    ""from"": ""now-6h"",
    ""to"": ""now""
  },
  ""timepicker"": {
    ""refresh_intervals"": [
      ""5s"",
      ""10s"",
      ""30s"",
      ""1m"",
      ""5m"",
      ""15m"",
      ""30m"",
      ""1h"",
      ""2h"",
      ""1d""
    ]
  },
  ""timezone"": """",
  ""title"": """ + dashboardTitle + @""",
  ""variables"": {
    ""list"": []
  },
  ""version"": 9
},
      ""overwrite"":true,
      ""inputs"":[

      ],
      ""folderId"":" + folderId.ToString() + @",
      ""title"":""" + dashboardTitle + @"""
   }";
            return await this.ImportNewDashboard(dashboardTemplate);
        }

        /// <inheritdoc/>
        public async Task<GrafanaDashboardDto> ImportSecurityDashboard(long folderId, string dashboardTitle, string workspaceGroupName)
        {
            if (folderId <= 0 || string.IsNullOrEmpty(dashboardTitle))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var dashboardTemplate = @"{
   ""dashboard"":{
   ""annotations"":{
      ""list"":[
         {
            ""builtIn"":1,
            ""datasource"":""-- Grafana --"",
            ""enable"":true,
            ""hide"":true,
            ""iconColor"":""rgba(0, 211, 255, 1)"",
            ""name"":""Annotations & Alerts"",
            ""type"":""dashboard""
         }
      ]
   },
   ""editable"":true,
   ""gnetId"":null,
   ""graphTooltip"":0,
   ""iteration"":1600685682730,
   ""links"":[
      
   ],
   ""panels"":[
      {
         ""datasource"":""wazuh-alerts"",
         ""gridPos"":{
            ""h"":7,
            ""w"":4,
            ""x"":0,
            ""y"":0
         },
         ""id"":11,
         ""options"":{
            ""colorMode"":""value"",
            ""fieldOptions"":{
               ""calcs"":[
                  ""mean""
               ],
               ""defaults"":{
                  ""mappings"":[
                     
                  ],
                  ""thresholds"":{
                     ""mode"":""absolute"",
                     ""steps"":[
                        {
                           ""color"":""green"",
                           ""value"":null
                        },
                        {
                           ""color"":""red"",
                           ""value"":100
                        }
                     ]
                  }
               },
               ""overrides"":[
                  
               ],
               ""values"":false
            },
            ""graphMode"":""area"",
            ""justifyMode"":""auto"",
            ""orientation"":""auto""
         },
         ""pluginVersion"":""6.7.1"",
         ""targets"":[
            {
               ""bucketAggs"":[
                  {
                     ""field"":""timestamp"",
                     ""id"":""2"",
                     ""settings"":{
                        ""interval"":""1d"",
                        ""min_doc_count"":0,
                        ""trimEdges"":0
                     },
                     ""type"":""date_histogram""
                  }
               ],
               ""metrics"":[
                  {
                     ""field"":""select field"",
                     ""id"":""1"",
                     ""type"":""count""
                  }
               ],
               ""query"":""agent.name: $Agent_Name"",
               ""refId"":""A"",
               ""timeField"":""timestamp""
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Số các sự kiện trong ngày"",
         ""type"":""stat""
      },
      {
         ""aliasColors"":{
            ""High Level Event"":""red"",
            ""Low Level Event"":""green"",
            ""Mức ảnh hưởng cao"":""red"",
            ""Mức ảnh hưởng thấp"":""green""
         },
         ""bars"":true,
         ""dashLength"":10,
         ""dashes"":false,
         ""datasource"":""wazuh-alerts"",
         ""fill"":1,
         ""fillGradient"":0,
         ""gridPos"":{
            ""h"":7,
            ""w"":20,
            ""x"":4,
            ""y"":0
         },
         ""hiddenSeries"":false,
         ""id"":2,
         ""legend"":{
            ""avg"":false,
            ""current"":false,
            ""max"":false,
            ""min"":false,
            ""show"":true,
            ""total"":false,
            ""values"":false
         },
         ""lines"":false,
         ""linewidth"":1,
         ""nullPointMode"":""null"",
         ""options"":{
            ""dataLinks"":[
               
            ]
         },
         ""percentage"":false,
         ""pointradius"":2,
         ""points"":false,
         ""renderer"":""flot"",
         ""seriesOverrides"":[
            
         ],
         ""spaceLength"":10,
         ""stack"":false,
         ""steppedLine"":false,
         ""targets"":[
            {
               ""bucketAggs"":[
                  {
                     ""fake"":true,
                     ""id"":""3"",
                     ""query"":""*"",
                     ""settings"":{
                        ""filters"":[
                           {
                              ""label"":""Mức ảnh hưởng thấp"",
                              ""query"":""rule.level: [0 TO 9]""
                           },
                           {
                              ""label"":""Mức ảnh hưởng cao"",
                              ""query"":""rule.level: [10 TO *]""
                           }
                        ]
                     },
                     ""type"":""filters""
                  },
                  {
                     ""field"":""timestamp"",
                     ""id"":""2"",
                     ""settings"":{
                        ""interval"":""1h"",
                        ""min_doc_count"":0,
                        ""trimEdges"":0
                     },
                     ""type"":""date_histogram""
                  }
               ],
               ""metrics"":[
                  {
                     ""field"":""select field"",
                     ""id"":""1"",
                     ""type"":""count""
                  }
               ],
               ""query"":""agent.name: $Agent_Name"",
               ""refId"":""A"",
               ""timeField"":""timestamp""
            }
         ],
         ""thresholds"":[
            
         ],
         ""timeFrom"":null,
         ""timeRegions"":[
            
         ],
         ""timeShift"":null,
         ""title"":""Số lượng các sự kiện theo thời gian"",
         ""tooltip"":{
            ""shared"":true,
            ""sort"":0,
            ""value_type"":""individual""
         },
         ""type"":""graph"",
         ""xaxis"":{
            ""buckets"":null,
            ""mode"":""time"",
            ""name"":null,
            ""show"":true,
            ""values"":[
               
            ]
         },
         ""yaxes"":[
            {
               ""format"":""short"",
               ""label"":null,
               ""logBase"":1,
               ""max"":null,
               ""min"":null,
               ""show"":true
            },
            {
               ""format"":""short"",
               ""label"":null,
               ""logBase"":1,
               ""max"":null,
               ""min"":null,
               ""show"":true
            }
         ],
         ""yaxis"":{
            ""align"":false,
            ""alignLevel"":null
         }
      },
      {
         ""columns"":[
            
         ],
         ""datasource"":""wazuh-alerts"",
         ""fontSize"":""100%"",
         ""gridPos"":{
            ""h"":10,
            ""w"":18,
            ""x"":0,
            ""y"":7
         },
         ""id"":3,
         ""pageSize"":null,
         ""showHeader"":true,
         ""sort"":{
            ""col"":0,
            ""desc"":true
         },
         ""styles"":[
            {
               ""alias"":""Time"",
               ""align"":""auto"",
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""pattern"":""Time"",
               ""type"":""date""
            },
            {
               ""alias"":""Tên sự kiện"",
               ""align"":""auto"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":""rule.description"",
               ""thresholds"":[
                  
               ],
               ""type"":""string"",
               ""unit"":""short""
            },
            {
               ""alias"":""Số lần xảy ra"",
               ""align"":""auto"",
               ""colorMode"":""cell"",
               ""colors"":[
                  ""rgba(50, 172, 45, 0.97)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(245, 54, 54, 0.9)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":""Count"",
               ""thresholds"":[
                  ""10"",
                  ""50""
               ],
               ""type"":""number"",
               ""unit"":""short""
            },
            {
               ""alias"":""Mức độ ảnh hưởng"",
               ""align"":""auto"",
               ""colorMode"":""cell"",
               ""colors"":[
                  ""rgba(50, 172, 45, 0.97)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(245, 54, 54, 0.9)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":""rule.level"",
               ""thresholds"":[
                  ""7"",
                  ""10""
               ],
               ""type"":""number"",
               ""unit"":""short""
            },
            {
               ""alias"":"""",
               ""align"":""left"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""decimals"":2,
               ""pattern"":""/.*/"",
               ""thresholds"":[
                  
               ],
               ""type"":""number"",
               ""unit"":""short""
            }
         ],
         ""targets"":[
            {
               ""bucketAggs"":[
                  {
                     ""fake"":true,
                     ""field"":""rule.level"",
                     ""id"":""3"",
                     ""settings"":{
                        ""min_doc_count"":1,
                        ""order"":""desc"",
                        ""orderBy"":""_count"",
                        ""size"":""10""
                     },
                     ""type"":""terms""
                  },
                  {
                     ""field"":""rule.description"",
                     ""id"":""2"",
                     ""settings"":{
                        ""min_doc_count"":1,
                        ""order"":""desc"",
                        ""orderBy"":""_term"",
                        ""size"":""10""
                     },
                     ""type"":""terms""
                  }
               ],
               ""metrics"":[
                  {
                     ""field"":""select field"",
                     ""id"":""1"",
                     ""type"":""count""
                  }
               ],
               ""query"":""agent.name: $Agent_Name"",
               ""refId"":""A"",
               ""timeField"":""timestamp""
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Các sự kiện và mức độ ảnh hưởng"",
         ""transform"":""table"",
         ""type"":""table""
      },
      {
         ""cacheTimeout"":null,
         ""columns"":[
            
         ],
         ""datasource"":""wazuh-alerts"",
         ""fontSize"":""100%"",
         ""gridPos"":{
            ""h"":10,
            ""w"":6,
            ""x"":18,
            ""y"":7
         },
         ""id"":9,
         ""links"":[
            
         ],
         ""pageSize"":null,
         ""showHeader"":true,
         ""sort"":{
            ""col"":1,
            ""desc"":true
         },
         ""styles"":[
            {
               ""alias"":""Time"",
               ""align"":""auto"",
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""pattern"":""Time"",
               ""type"":""date""
            },
            {
               ""alias"":"""",
               ""align"":""auto"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":"""",
               ""thresholds"":[
                  
               ],
               ""type"":""number"",
               ""unit"":""short""
            },
            {
               ""alias"":""Tên nhóm"",
               ""align"":""left"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""decimals"":2,
               ""pattern"":""rule.groups"",
               ""thresholds"":[
                  
               ],
               ""type"":""string"",
               ""unit"":""short""
            },
            {
               ""alias"":""Số lần xảy ra"",
               ""align"":""left"",
               ""colorMode"":""cell"",
               ""colors"":[
                  ""rgba(50, 172, 45, 0.97)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(245, 54, 54, 0.9)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":""Count"",
               ""thresholds"":[
                  ""100"",
                  ""200""
               ],
               ""type"":""number"",
               ""unit"":""short""
            }
         ],
         ""targets"":[
            {
               ""bucketAggs"":[
                  {
                     ""field"":""rule.groups"",
                     ""id"":""2"",
                     ""settings"":{
                        ""min_doc_count"":1,
                        ""order"":""desc"",
                        ""orderBy"":""_count"",
                        ""size"":""10""
                     },
                     ""type"":""terms""
                  }
               ],
               ""metrics"":[
                  {
                     ""field"":""select field"",
                     ""id"":""1"",
                     ""type"":""count""
                  }
               ],
               ""query"":""agent.name: $Agent_Name"",
               ""refId"":""A"",
               ""timeField"":""timestamp""
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Nhóm các sự kiện"",
         ""transform"":""table"",
         ""type"":""table""
      },
      {
         ""columns"":[
            {
               ""text"":""rule.level"",
               ""value"":""rule.level""
            },
            {
               ""text"":""rule.description"",
               ""value"":""rule.description""
            },
            {
               ""text"":""full_log"",
               ""value"":""full_log""
            },
            {
               ""text"":""timestamp"",
               ""value"":""timestamp""
            }
         ],
         ""datasource"":""wazuh-alerts"",
         ""fontSize"":""100%"",
         ""gridPos"":{
            ""h"":11,
            ""w"":24,
            ""x"":0,
            ""y"":17
         },
         ""id"":7,
         ""pageSize"":null,
         ""showHeader"":true,
         ""sort"":{
            ""col"":3,
            ""desc"":true
         },
         ""styles"":[
            {
               ""alias"":""Time"",
               ""align"":""auto"",
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""pattern"":""Time"",
               ""type"":""date""
            },
            {
               ""alias"":"""",
               ""align"":""auto"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":"""",
               ""thresholds"":[
                  
               ],
               ""type"":""number"",
               ""unit"":""short""
            },
            {
               ""alias"":""Mức độ ảnh hưởng"",
               ""align"":""center"",
               ""colorMode"":""cell"",
               ""colors"":[
                  ""rgba(50, 172, 45, 0.97)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(245, 54, 54, 0.9)""
               ],
               ""decimals"":0,
               ""pattern"":""rule.level"",
               ""thresholds"":[
                  ""7"",
                  ""10""
               ],
               ""type"":""number"",
               ""unit"":""short""
            },
            {
               ""alias"":""Tên sự kiện"",
               ""align"":""right"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":""rule.description"",
               ""thresholds"":[
                  
               ],
               ""type"":""string"",
               ""unit"":""short""
            },
            {
               ""alias"":""Chi tiết"",
               ""align"":""left"",
               ""colorMode"":null,
               ""colors"":[
                  ""rgba(245, 54, 54, 0.9)"",
                  ""rgba(237, 129, 40, 0.89)"",
                  ""rgba(50, 172, 45, 0.97)""
               ],
               ""dateFormat"":""YYYY-MM-DD HH:mm:ss"",
               ""decimals"":2,
               ""mappingType"":1,
               ""pattern"":""data.win.system.message"",
               ""thresholds"":[
                  
               ],
               ""type"":""string"",
               ""unit"":""short""
            }
         ],
         ""targets"":[
            {
               ""bucketAggs"":[
                  
               ],
               ""metrics"":[
                  {
                     ""field"":""select field"",
                     ""id"":""1"",
                     ""meta"":{
                        
                     },
                     ""settings"":{
                        ""size"":500
                     },
                     ""type"":""raw_document""
                  }
               ],
               ""query"":""agent.name: $Agent_Name AND !decoder.name: sca"",
               ""refId"":""A"",
               ""timeField"":""timestamp""
            }
         ],
         ""timeFrom"":null,
         ""timeShift"":null,
         ""title"":""Chi tiết về các sự kiện"",
         ""transform"":""json"",
         ""type"":""table""
      }
   ],
   ""schemaVersion"":22,
   ""style"":""dark"",
   ""tags"":[
      
   ],
   ""templating"":{
      ""list"":[
         {
            ""allValue"":null,
            ""current"":{
               ""selected"":false
            },
            ""datasource"":""wazuh-monitoring"",
            ""definition"":""{\""find\"": \""terms\"", \""field\"": \""name\"", \""query\"": \""group:" + workspaceGroupName + @"\""}"",
            ""hide"":0,
            ""includeAll"":false,
            ""index"":-1,
            ""label"":null,
            ""multi"":false,
            ""name"":""Agent_Name"",
            ""options"":[
               
            ],
            ""query"":""{\""find\"": \""terms\"", \""field\"": \""name\"", \""query\"": \""group:" + workspaceGroupName + @"\""}"",
            ""refresh"":2,
            ""regex"":"""",
            ""skipUrlSync"":false,
            ""sort"":1,
            ""tagValuesQuery"":"""",
            ""tags"":[
               
            ],
            ""tagsQuery"":"""",
            ""type"":""query"",
            ""useTags"":false
         }
      ]
   },
   ""time"":{
      ""from"":""now-24h"",
      ""to"":""now""
   },
   ""timepicker"":{
      ""refresh_intervals"":[
         ""5s"",
         ""10s"",
         ""30s"",
         ""1m"",
         ""5m"",
         ""15m"",
         ""30m"",
         ""1h"",
         ""2h"",
         ""1d""
      ]
   },
   ""timezone"":"""",
   ""title"":""" + dashboardTitle + @""",
   ""variables"":{
      ""list"":[
         
      ]
   },
   ""version"":8
},
   ""overwrite"":true,
   ""inputs"":[

   ],
   ""folderId"":" + folderId.ToString() + @",
   ""title"":""" + dashboardTitle + @"""
}";
            return await this.ImportNewDashboard(dashboardTemplate);
        }

        /// <inheritdoc/>
        public async Task<bool> AssignPermissionToUserByEmail(string email, long dashboardId)
        {
            // Get user id by email.
            var grafanaAccountDto = await this.GetGrafanaAccountDetail(email);

            if (grafanaAccountDto == null || grafanaAccountDto.Id <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            // Get permission list.
            var permissions = await this.GetDashboardPermissions(dashboardId);

            // Set new permission record.
            var newPermissions = new List<GrafanaPermissionDto>();
            foreach (var el in permissions)
            {
                if (el.UserId == grafanaAccountDto.Id)
                {
                    return true;
                }

                if (el.TeamId <= 0)
                {
                    newPermissions.Add(el);
                }
            }

            // Update dashboard permission.
            newPermissions.Add(new GrafanaPermissionDto
            {
                UserId = grafanaAccountDto.Id,
                Permission = (int)EnGrafanaPermission.View,
            });

            return await this.UpdateDashboardPermission(newPermissions, dashboardId);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GrafanaPermissionDto>> GetDashboardPermissions(long dashboardId)
        {
            if (dashboardId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var url = $"{this.baseUrl}/api/dashboards/id/" + dashboardId.ToString() + "/permissions";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var response = await this.SendRequest(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<GrafanaPermissionDto>>(responseBody);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveDashboardPermission(string email, long dashboardId)
        {
            // Get user id by email.
            var grafanaAccountDto = await this.GetGrafanaAccountDetail(email);

            if (grafanaAccountDto == null || grafanaAccountDto.Id <= 0)
            {
                return true;
            }

            // Get permission list.
            var permissions = await this.GetDashboardPermissions(dashboardId);

            // Set new permission record.
            var newPermissions = new List<GrafanaPermissionDto>();

            var grafanaPermissionDtos = permissions.ToList();
            if (!grafanaPermissionDtos.Any(p => p.UserId == grafanaAccountDto.Id))
            {
                return true;
            }

            foreach (var el in grafanaPermissionDtos)
            {
                if (el.TeamId <= 0 && el.UserId != grafanaAccountDto.Id)
                {
                    newPermissions.Add(el);
                }
            }

            return await this.UpdateDashboardPermission(newPermissions, dashboardId);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateDashboardPermission(IEnumerable<GrafanaPermissionDto> permissions, long dashboardId)
        {
            var url = $"{this.baseUrl}/api/dashboards/id/" + dashboardId.ToString() + "/permissions";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, (permissions == null || !permissions.Any()) ? "{\"items\": []}" : "{\"items\":" + JsonConvert.SerializeObject(permissions) + "}");
            var response = await this.SendRequest(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateGrafanaAccount(GrafanaAccountDto account)
        {
            if (account == null || string.IsNullOrEmpty(account.Login)
                                || string.IsNullOrEmpty(account.Password) || string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Name))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var existingAccount = await this.GetGrafanaAccountDetail(account.Email);

            var defaultOrg = await this.GetAdminAccountDefaultOrganization();

            if (existingAccount != null && existingAccount.Id > 0)
            {
                // if exiting account does not belong to the default org
                if (defaultOrg != null && defaultOrg.Id > 0 && existingAccount.OrgId != defaultOrg.Id)
                {
                    await this.AddUserToOrganization(existingAccount.Email);
                }

                // assign the exiting account to the org.
                return true;
            }

            var url = $"{this.baseUrl}/api/admin/users";

            var jsonObject = @"{
                  ""name"":""" + account.Name + @""",
                  ""email"":""" + account.Email + @""",
                  ""login"":""" + account.Login + @""",
                  ""password"":""" + account.Password + @""",
                  ""OrgId"": " + (defaultOrg != null && defaultOrg.Id > 0 ? defaultOrg.Id : (int)EnGrafanaOrganizationIds.VadarOrg).ToString() + @"
                }";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, jsonObject);
            var response = await this.SendRequest(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <inheritdoc/>
        public async Task AddUserToOrganization(string userEmail)
        {
            var url = $"{this.baseUrl}/api/org/invites";

            var request = this.HttpRequestMessage(HttpMethod.Post, url, "{\"loginOrEmail\":\"" + userEmail + "\",\"role\":\"Viewer\",\"sendEmail\":false}");
            var response = await this.SendRequest(request);
            await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc/>
        public async Task<GrafanaOrganizationDto> GetAdminAccountDefaultOrganization()
        {
            var url = $"{this.baseUrl}/api/org";

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var response = await this.SendRequest(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GrafanaOrganizationDto>(responseBody);
        }

        /// <inheritdoc/>
        public async Task<GrafanaAccountDto> GetGrafanaAccountDetail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new VadarException(ErrorCode.ArgumentNull);
            }

            var url = $"{this.baseUrl}/api/users/lookup?loginOrEmail=" + email;

            var request = this.HttpRequestMessage(HttpMethod.Get, url);
            var response = await this.SendRequest(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GrafanaAccountDto>(responseBody);
        }

        /// <inheritdoc/>
        public async Task<dynamic> LogoutAllDevices(long grafanaAccountId)
        {
            if (grafanaAccountId <= 0)
            {
                throw new VadarException(ErrorCode.ArgumentInvalid);
            }

            var url = $"{this.baseUrl}/api/admin/users/" + grafanaAccountId.ToString() + "/logout";

            var request = this.HttpRequestMessage(HttpMethod.Post, url);
            var response = await this.SendRequest(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<dynamic>(responseBody);
        }

        /// <summary>
        /// Import New Dashboard.
        /// </summary>
        /// <param name="dashboardTemplate">Dashboard template.</param>
        /// <returns>Grafana Dashboard Dto.</returns>
        protected async Task<GrafanaDashboardDto> ImportNewDashboard(string dashboardTemplate)
        {
            var url = $"{this.baseUrl}/api/dashboards/import";
            var request = this.HttpRequestMessage(HttpMethod.Post, url, dashboardTemplate);
            var response = await this.SendRequest(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GrafanaDashboardDto>(responseBody);
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

        private string CredentialsEnCoded()
        {
            return Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(this.configs["Grafana:UserName"] + ":" + this.configs["Grafana:Password"]));
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            using var client = new HttpClient(this.HttpClientHandler());

            // client.DefaultRequestHeaders.Remove("Content-Type");
            // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            // client.DefaultRequestHeaders. .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            return await client.SendAsync(request);

            // var responseBody = await response.Content.ReadAsStringAsync();
            // return responseBody;
        }

        private HttpClientHandler HttpClientHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
            };
        }
    }
}
