// <copyright file="Startup.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Mapping;
using VADAR.Model;
using VADAR.WebAPI.Attributes.Filter;
using VADAR.WebApi.Modules;
using VADAR.WebAPI.Modules;

namespace VADAR
{
    /// <summary>
    /// Startup class for api application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Startup"/> class.
        /// Public contructor method.
        /// </summary>
        /// <param name="configuration">configuration object.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets application container.
        /// </summary>
        public IContainer ApplicationContainer { get; private set; }

        /// <summary>
        /// Gets configuration object.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">service collection. </param>
        /// <returns>service provider.</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddMemoryCache();

            // _ = services.AddAutoMapper(c => c.AddProfile(new DtoProfile()));
            services.AddAutoMapper(typeof(DtoProfile));
            services.AddSingleton(this.Configuration);
            services.AddScoped<VADARExceptionFilter>();
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyOrigin()
                    .WithOrigins(this.Configuration.GetSection("CORS-Settings:Allow-Origins").Get<string[]>());
                });
            });

            // This settings is for reference token
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = this.Configuration["IdentityServerSetting:IdentityServerUrl"];
                    options.ApiName = this.Configuration["IdentityServerSetting:ValidAudience"];
                    options.ApiSecret = this.Configuration["IdentityServerSetting:ApiSecret"];

                    // options.TokenRetriever = TokenReceiveHelper.FromHeaderAndQueryString;
                    options.BackChannelTimeouts = TimeSpan.FromMinutes(30);
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = this.Configuration["RedisConnectionString"];
                options.InstanceName = "VSECBusinessApiInstance";
            });

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IRazorViewHelper, RazorViewHelper>();
            services.AddScoped<IRecaptchaHelper, RecaptchaHelper>();
            services.AddAuthorization();
            services.AddDbContext<VADARDbContext>();
            services.AddControllersWithViews(config =>
            {
                config.EnableEndpointRouting = false;

                // config.Filters.Add(new UserDataFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddMailKit(optionBuilder =>
            {
                optionBuilder.UseMailKit(new MailKitOptions
                {
                    // get options from sercets.json
                    Server = this.Configuration["EmailSetting:Server"],
                    Port = Convert.ToInt32(this.Configuration["EmailSetting:Port"]),
                    SenderName = this.Configuration["EmailSetting:SenderName"],
                    SenderEmail = this.Configuration["EmailSetting:SenderEmail"],

                    // enable ssl or tls
                    Security = true,
                });
            });

            if (this.Configuration.GetSection("EnableSwagger").Get<bool>())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo { Title = "VADAR.WebAPI", Description = "Core API", Version = "v1" });
                    c.DescribeAllParametersInCamelCase();

                    // Add JWT Bearer Authorization
                    c.AddSecurityDefinition(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                        });

                    // c.AddSecurityRequirement(security);
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new string[] { }
                        },
                    });

                    // Sorting all actions
                    // c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

                    // var basePath = AppContext.BaseDirectory;
                    // var xmlPath = Path.Combine(basePath, "VADAR.WebAPI.xml");

                    // c.IncludeXmlComments(xmlPath);
                });
            }

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new EfModule());
            builder.RegisterModule(new LoggerModule());
            builder.RegisterModule(new MappingModule());
            builder.RegisterModule(new UnitOfWorkModule());
            builder.RegisterModule(new HelperModule());
            this.ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment variable.</param>
        /// <param name="loggerFactory">Logger Factory.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.RollingFile("logs/log-{Date}.log").CreateLogger();
            loggerFactory.WithFilter(new FilterLoggerSettings
                {
                    { "Trace", LogLevel.Trace },
                    { "Default", LogLevel.Trace },
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning },
                }).AddSerilog();

            app.UseCors("default");
            app.UseStatusCodePages();

            if (this.Configuration.GetSection("EnableSwagger").Get<bool>())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VADAR.WebAPI v1");
                    c.DocumentTitle = "Title Documentation";
                    c.DocExpansion(DocExpansion.None);
                });
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
