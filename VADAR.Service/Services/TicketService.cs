// <copyright file="TicketService.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VADAR.DTO;
using VADAR.Exceptions;
using VADAR.Helpers;
using VADAR.Helpers.Interfaces;
using VADAR.Repository.UnitOfWork.Interfaces;
using VADAR.Service.Interfaces;

namespace VADAR.Service.Services
{
    /// <summary>
    /// User service class.
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly IMapper mapper;
        private readonly IConfiguration configs;
        private readonly IRedisCachingHelper redisCachingHelper;
        private readonly IWorkspaceUnitOfWork unitOfWork;
        private string url;

        /// <summary>
        /// Initialises a new instance of the <see cref="TicketService"/> class.
        /// </summary>
        /// <param name="mapper">mapper.</param>
        /// <param name="configs">configs.</param>
        /// <param name="redisCachingHelper">redisCachingHelper.</param>
        /// <param name="unitOfWork">unitOfWork.</param>
        public TicketService(IMapper mapper, IConfiguration configs, IRedisCachingHelper redisCachingHelper, IWorkspaceUnitOfWork unitOfWork)
        {
            Guard.IsNotNull(mapper, nameof(mapper));
            this.mapper = mapper;
            this.configs = configs;
            this.redisCachingHelper = redisCachingHelper;
            this.unitOfWork = unitOfWork;
            this.url = $"{this.configs["TheHiveUrl"]}/api";
        }

        /// <inheritdoc/>
        public async Task<TicketDto> Index(TicketDto ticket)
        {
            // Here I want to call the method named n in the dynamic d
            MethodInfo method = this.GetType().GetMethod(ticket.Services);
            if (method != null)
            {
                object[] parameters = new object[]
                {
                    ticket,
                };
                Task<TicketDto> a = (Task<TicketDto>)method.Invoke(this, parameters);
                return await a;
            }

            return new TicketDto();
        }

        /// <inheritdoc/>
        public async Task<TicketDto> GetTicketElasticsearch(TicketDto ticket)
        {
            if (ticket.Query.pageSize == null)
            {
                ticket.Query.pageSize = 10;
            }

            if (ticket.Query.pageIndex == null)
            {
                ticket.Query.pageIndex = 1;
            }

            if (ticket.Query.gte == null)
            {
                ticket.Query.gte = DateTime.Now.AddDays(-1);
            }

            if (ticket.Query.lte == null)
            {
                ticket.Query.lte = DateTime.Now;
            }

            var arrsTitle = this.ReplateSpecial((string)ticket.Query.title).Split("-");
            string title = string.Empty;
            for (int i = 0; i < arrsTitle.Length; i++)
            {
                if (i > 0)
                {
                    title += ",";
                }

                title += @"{
                    ""wildcard"": {
                        ""title"": ""*" + arrsTitle[i] + @"*""
                    }
                }";
            }

            var arrsOwner = this.ReplateSpecial((string)ticket.Owner).Split("-");
            string owner = string.Empty;
            for (int i = 0; i < arrsOwner.Length; i++)
            {
                if (i > 0)
                {
                    owner += ",";
                }

                owner += @"{
                    ""wildcard"": {
                        ""createBy"": ""*" + arrsOwner[i] + @"*""
                    }
                }";
            }

            var baseUrl = $"{this.configs["ElasticSeachUrl"]}/thehive/_search";
            var body = @"{
                ""sort"": [
                    {
                        ""timestamp"": {
                            ""order"": ""desc"",
                            ""unmapped_type"": ""boolean""
                        }
                    }
                ],
                ""from"": """ + ((ticket.Query.pageIndex - 1) * ticket.Query.pageSize) + @""",
                ""size"": """ + ticket.Query.pageSize + @""",
                ""_source"": {
                    ""excludes"": []
                },
                ""track_total_hits"": true,
                ""query"": {
                    ""bool"": {
                        ""must"": [" +
                            title
                        + @"],
                        ""filter"": [" +
                            owner + @",
                            {
                                ""match_phrase"": {
                                    ""workspaceId"": """ + ticket.WorkspaceId + @"""
                                }
                            },"
                            + (!string.IsNullOrEmpty(ticket.Query.status.Value) ?
                            @"{
                                ""match_phrase"": {
                                    ""status"": """ + ticket.Query.status.Value + @"""
                                }
                            }," : string.Empty)
                            +
                            @"{
                                ""range"": {
                                    ""timestamp"": {
                                    ""gte"": """ + ticket.Query.gte.Value.ToUniversalTime().ToString("o") + @""",
                                    ""lte"": """ + ticket.Query.lte.Value.ToUniversalTime().ToString("o") + @""",
                                    ""format"": ""strict_date_optional_time""
                                    }
                                }
                            }
                        ]
                    }
                }
            }";

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = this.GetMethod(ticket.Method),
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            var data = ticket;
            data.Result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            data.Query = new { };
            data.IsSuccessStatusCode = response.IsSuccessStatusCode;

            return data;
        }

        /// <inheritdoc/>
        public async Task<TicketDto> Ticket(TicketDto ticket)
        {
            // login thehive
            if (ticket.Index == "login")
            {
                ticket.Query = JsonConvert.DeserializeObject<dynamic>("{\"user\": \"" + this.configs["TheHiveU"] + "\", \"password\":\"" + this.configs["TheHiveP"] + "\"}");
            }

            var url = $"{this.url}/{ticket.Index}";
            var cookieId = "THEHIVE-SESSION";
            Uri uri = new Uri(url);
            CookieContainer cookies = new CookieContainer();

            // query thehive with cookie
            if (ticket.UseCookie)
            {
                var valueRedis = await this.redisCachingHelper.GetDataByKey(cookieId);
                if (!string.IsNullOrEmpty(valueRedis))
                {
                    cookies.Add(uri, new Cookie(cookieId, valueRedis));
                }
            }

            // add cookie
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.configs["TheHiveToken"]}");
            if (!string.IsNullOrEmpty(ticket.Organisation))
            {
                client.DefaultRequestHeaders.Add("X-Organisation", ticket.Organisation);
            }

            var request = new HttpRequestMessage
            {
                Method = this.GetMethod(ticket.Method),
                RequestUri = new Uri(url),
                Content = new StringContent(JsonConvert.SerializeObject(ticket.Query), Encoding.UTF8, "application/json"),
            };
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (ticket.Index == "login")
            {
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
                foreach (Cookie cookie in responseCookies)
                {
                    if (cookie.Name == cookieId)
                    {
                        // get cookie
                        _ = this.redisCachingHelper.SetStringData(cookieId, cookie.Value, 60);
                    }
                }
            }

            var responseStatus = response.IsSuccessStatusCode;
            var data = ticket;
            data.Result = JsonConvert.DeserializeObject<dynamic>(responseBody);

            if (ticket.Index == "organisation" && responseStatus)
            {
                var isUpdate = await this.UpdateWorkspace(ticket.WorkspaceId, data.Result.id.Value);
            }

            var sp = ticket.Index.Split("/");
            if (sp[0] == "case" && responseStatus)
            {
                if (sp.Length < 2)
                {
                    responseStatus = await this.QueryTicketElasticsearch(ticket.Owner, ticket.Query, data.Result.id.Value, ticket.WorkspaceId, "create");
                }
                else
                {
                    responseStatus = await this.QueryTicketElasticsearch(ticket.Owner, ticket.Query, sp[1], ticket.WorkspaceId, "update");
                }
            }

            data.Query = new { };
            data.IsSuccessStatusCode = responseStatus;

            return data;
        }

        private string ReplateSpecial(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return Regex.Replace(str, "[^a-zA-Z0-9_]+", "-");
        }

        private async Task<bool> UpdateWorkspace(int wpId, string id)
        {
            var workspace = await this.unitOfWork.WorkspaceRepository.GetWorkspaceById(wpId);
            if (workspace == null)
            {
                throw new VadarException(ErrorCode.WorkspaceNull, nameof(ErrorCode.WorkspaceNull));
            }

            workspace.OrgId = id;

            await this.unitOfWork.WorkspaceRepository.Edit(workspace);
            return await this.unitOfWork.Commit() > 0;
        }

        private async Task<bool> QueryTicketElasticsearch(string owner, dynamic query, string uid, int workspaceId, string action)
        {
            var baseUrl = string.Empty;
            var body = string.Empty;
            var method = HttpMethod.Put;

            if (action == "create")
            {
                baseUrl = $"{this.configs["ElasticSeachUrl"]}/thehive/_create/{uid}";
                body = @"{
                    ""title"": """ + query.title + @""",
                    ""severity"": " + (query.severity != null ? query.severity : 2) + @",
                    ""thehiveId"": """ + uid + @""",
                    ""timestamp"": """ + DateTime.Now.ToUniversalTime().ToString("o") + @""",
                    ""createBy"": """ + owner + @""",
                    ""workspaceId"": """ + workspaceId + @""",
                    ""endDate"": """",
                    ""status"": ""Open""
                }";
            }
            else
            {
                baseUrl = $"{this.configs["ElasticSeachUrl"]}/thehive/_doc/{uid}/_update";

                var status = string.Empty;
                var title = string.Empty;
                var severity = string.Empty;
                var timestamp = string.Empty;
                var endDate = string.Empty;

                if (!string.IsNullOrEmpty((string)query.status))
                {
                    status = query.status;
                }

                if (!string.IsNullOrEmpty((string)query.title))
                {
                    title = query.title;
                }

                if (!string.IsNullOrEmpty((string)query.severity))
                {
                    severity = query.severity;
                }

                if (!string.IsNullOrEmpty((string)query.startDate))
                {
                    timestamp = this.ToDateTime(Convert.ToDouble((string)query.startDate)).ToUniversalTime().ToString("o");
                }

                if (!string.IsNullOrEmpty((string)query.endDate))
                {
                    endDate = this.ToDateTime(Convert.ToDouble((string)query.endDate)).ToUniversalTime().ToString("o");
                }

                var doc = new
                {
                    doc = new
                    {
                        status,
                        title,
                        severity,
                        timestamp,
                        endDate,
                    },
                };

                body = JsonConvert.SerializeObject(doc, new
                JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });

                body = this.Pretreatment(body);
                method = HttpMethod.Post;
            }

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(baseUrl),
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        private string Pretreatment(string obj)
        {
            var temp = JObject.Parse(obj);
            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Value.ToString() == string.Empty)
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes
            return temp.ToString();
        }

        private DateTime ToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private dynamic GetMethod(string method)
        {
            var m = HttpMethod.Get;
            switch (method.ToUpper())
            {
                case "POST":
                {
                    m = HttpMethod.Post;
                    break;
                }

                case "PATCH":
                {
                    m = HttpMethod.Patch;
                    break;
                }

                case "DELETE":
                {
                    m = HttpMethod.Delete;
                    break;
                }
            }

            return m;
        }
    }
}
