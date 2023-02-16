using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using VADAR.Helpers.Helper;
using VADAR.Helpers.Interfaces;
using VADAR.Mapping;
using VADAR.NotificationApi.Attributes.Filter;
using VADAR.NotificationApi.Modules;

namespace VADAR.NotificationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets application container.
        /// </summary>
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DtoProfile));
            services.AddSingleton(Configuration);
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
                    .WithOrigins(Configuration.GetSection("CORS-Settings:Allow-Origins").Get<string[]>());
                });
            });

            // This settings is for reference token
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["IdentityServerSetting:IdentityServerUrl"];
                    options.ApiName = Configuration["IdentityServerSetting:ValidAudience"];
                    options.ApiSecret = Configuration["IdentityServerSetting:ApiSecret"];

                    // options.EnableCaching = true;
                    // options.CacheDuration = TimeSpan.FromMinutes(5);

                    // options.TokenRetriever = TokenReceiveHelper.FromHeaderAndQueryString;
                    options.BackChannelTimeouts = TimeSpan.FromMinutes(30);
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["RedisConnectionString"];
                options.InstanceName = "VSECNotificationApiInstance";
            });

            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IRazorViewHelper, RazorViewHelper>();
            services.AddScoped<IRecaptchaHelper, RecaptchaHelper>();
            services.AddAuthorization();

            //services.AddControllers();
            services.AddControllersWithViews(config =>
            {
                config.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Add MailKit
            services.AddMailKit(optionBuilder =>
            {
                optionBuilder.UseMailKit(new MailKitOptions
                {
                    //get options from sercets.json
                    Server = Configuration["EmailSetting:Server"],
                    Port = Convert.ToInt32(Configuration["EmailSetting:Port"]),
                    SenderName = Configuration["EmailSetting:SenderName"],
                    SenderEmail = Configuration["EmailSetting:SenderEmail"],

                    // can be optional with no authentication 
                    //Account = Configuration["Account"],
                    //Password = Configuration["Password"],
                    // enable ssl or tls
                    Security = true
                });
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VADAR.NotificationAPI", Description = "Core API", Version = "v1"});
                c.DescribeAllParametersInCamelCase();

                // Add JWT Bearer Authorization
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"", Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey });

                // c.AddSecurityRequirement(security);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                     },
                     new string[] { }
                    }
                });
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new EfModule());
            builder.RegisterModule(new LoggerModule());
            builder.RegisterModule(new MappingModule());
            builder.RegisterModule(new UnitOfWorkModule());
            builder.RegisterModule(new HelperModule());
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Log.Logger = new LoggerConfiguration().WriteTo.RollingFile("logs/log-{Date}.log").CreateLogger();
            loggerFactory.WithFilter(new FilterLoggerSettings
                {
                    { "Trace", LogLevel.Trace },
                    { "Default", LogLevel.Trace },
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning }
                }).AddSerilog();

            app.UseCors("default");
            app.UseStatusCodePages();

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VADAR.WebAPI v1");
                c.DocumentTitle = "Title Documentation";
                c.DocExpansion(DocExpansion.None);
            });

            app.UseRouting();

            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseMvc();
        }
    }
}
