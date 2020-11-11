using AutoMapper;
using System;
using Dapper;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Utility.Formatter;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry.Extensibility;
using Microsoft.AspNetCore.Mvc.Versioning;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FewBox.Core.Web.Sentry;
using FewBox.Core.Web.Orm;
using System.Collections.Generic;

namespace FewBox.Core.Web.Extension
{
    public static class FewBoxWebExtension
    {
        public static void AddFewBox(this IServiceCollection services, FewBoxDBType fewBoxDBType = FewBoxDBType.MySQL, FewBoxAuthType fewBoxAuthType = FewBoxAuthType.Payload, ApiVersion defaultVersion = default(ApiVersion), int responseCacheDuration = 3600)
        {
            // Init config.
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("./appsettings.json")
            .AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();
            var fewBoxConfig = configuration.GetSection("FewBox").Get<FewBoxConfig>();
            services.AddSingleton(fewBoxConfig);
            // Init env.
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IWebHostEnvironment webHostEnvironment = serviceProvider.GetService<IWebHostEnvironment>();
            // Switch env.
            if (webHostEnvironment.IsProduction())
            {
                RestfulUtility.IsCertificateNeedValidate = true; // Whether check the ceritfication.
                RestfulUtility.IsLogging = false; // Is logging request.
                HttpUtility.IsCertificateNeedValidate = true; // Whether check the ceritfication.
                services.AddScoped<IAuthService, RemoteAuthService>();
                if (fewBoxAuthType == FewBoxAuthType.Role)
                {
                    services.AddScoped<IAuthorizationHandler, RoleHandler>(); // Used for RBAC AOP.
                }
                else
                {
                    services.AddScoped<IAuthorizationHandler, PayloadHandler>(); // Used for RBAC AOP.
                }
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddGoogle(options =>
                {
                    options.ClientId = fewBoxConfig.Google.ClientId;
                    options.ClientSecret = fewBoxConfig.Google.ClientSecret;
                })
                .AddFacebook(options =>
                {
                    options.AppId = fewBoxConfig.Facebook.AppId;
                    options.AppSecret = fewBoxConfig.Facebook.AppSecret;
                })
                .AddTwitter(options =>
                {
                    options.ConsumerKey = fewBoxConfig.Twitter.ConsumerKey;
                    options.ConsumerSecret = fewBoxConfig.Twitter.ConsumerSecret;
                })
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = fewBoxConfig.MicrosoftAccount.ClientId;
                    options.ClientSecret = fewBoxConfig.MicrosoftAccount.ClientSecret;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = fewBoxConfig.JWT.Issuer,
                        ValidAudience = fewBoxConfig.JWT.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(fewBoxConfig.JWT.Key))
                    };
                });
            }
            else
            {
                RestfulUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
                RestfulUtility.IsLogging = true; // Is logging request.
                HttpUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
                services.AddScoped<IAuthService, StubeAuthService>();
            }
            // Switch db.
            if (fewBoxDBType == FewBoxDBType.MySQL)
            {
                services.AddScoped<IOrmSession, MySqlSession>();
            }
            else
            {
                SqlMapper.AddTypeHandler(new SQLiteGuidTypeHandler());
                services.AddScoped<IOrmSession, SQLiteSession>();
            }
            // Default
            JsonUtility.IsCamelCase = true; // Is camel case.
            JsonUtility.IsNullIgnore = true; // Ignore null json property.
            HttpUtility.IsEnsureSuccessStatusCode = false; // Whether ensure sucess status code.
            services.AddRouting(options => options.LowercaseUrls = true); // Lowercase the urls.
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("default", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = responseCacheDuration // One hour. [ResponseCache(CacheProfileName = "default", VaryByQueryKeys = new[] { "datetime" })]
                });
                options.Filters.Add<TransactionAsyncFilter>(); // Add DB transaction.
                options.Filters.Add<TraceAsyncFilter>(); // Add biz trace log.
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest);
            List<string> origins = new List<string> { "https://fewbox.com" };
            if (fewBoxConfig.Cors != null && fewBoxConfig.Cors.Origins != null && fewBoxConfig.Cors.Origins.Count > 0)
            {
                origins.AddRange(fewBoxConfig.Cors.Origins);
            }
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(origins.ToArray())
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                        });
                    options.AddPolicy("dev",
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins("http://localhost", "https://localhost")
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                        });

                });
            services.AddSingleton<IAuthorizationPolicyProvider, FewBoxAuthorizationPolicyProvider>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Auto Mapper.
            services.AddMemoryCache(); // Memory cache.
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            // Used for IHttpContextAccessor&IActionContextAccessor context.
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Used for JWT.
            services.AddScoped<ITokenService, JWTTokenService>();
            // Used for ApiVersion
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true; // Show versions in response.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.DefaultApiVersion = defaultVersion == null ? ApiVersion.Default : defaultVersion; // new ApiVersion(1, 0, "alpha");
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            // Used for Sentry
            services.AddTransient<ISentryEventProcessor, SentryEventProcessor>();
            services.AddSingleton<ISentryEventExceptionProcessor, SentryEventExceptionProcessor>();
            //  Used for SignalR
            services.AddSignalR();
            // Used for ORM.
            if (fewBoxConfig.Orm.InternalConnectionType == OrmConnectionType.Tenant)
            {
                services.AddScoped<IOrmConfiguration, AppSettingTenantOrmConfiguration>();
            }
            else
            {
                services.AddSingleton<IOrmConfiguration, AppSettingOrmConfiguration>();
            }
        }
    }
}