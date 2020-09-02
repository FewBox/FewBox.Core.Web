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
using Microsoft.AspNetCore.Mvc.Authorization;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using FewBox.Core.Web.Error;
using Microsoft.AspNetCore.Authorization;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using FewBox.Core.Web.Notification;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FewBox.Core.Web.Sentry;
using FewBox.Core.Web.Orm;

namespace FewBox.Core.Web.Extension
{
    public static class FewBoxExtension
    {
        public static void AddFewBox(this IServiceCollection services, FewBoxDBType fewBoxDBType = FewBoxDBType.MySQL)
        {
            // Init config.
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
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
                services.AddScoped<INotificationHandler, ServiceNotificationHandler>();
            }
            else
            {
                RestfulUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
                RestfulUtility.IsLogging = true; // Is logging request.
                HttpUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
                services.AddScoped<IAuthService, StubeAuthService>();
                services.AddScoped<INotificationHandler, ConsoleNotificationHandler>();
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
                if (webHostEnvironment.IsDevelopment())
                {
                    options.Filters.Add(new AllowAnonymousFilter()); // Ignore the authorization.
                }
                options.Filters.Add<TransactionAsyncFilter>(); // Add DB transaction.
                options.Filters.Add<TraceAsyncFilter>(); // Add biz trace log.
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddAutoMapper(typeof(FewBoxExtension)); // Auto Mapper.
            services.AddMemoryCache(); // Memory cache.
            services.AddSingleton<IExceptionProcessorService, ExceptionProcessorService>(); // Catch Exception.
            // Used for RBAC AOP.
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            services.AddScoped<ITryCatchService, TryCatchService>();
            // Used for IHttpContextAccessor&IActionContextAccessor context.
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Used for JWT.
            services.AddScoped<ITokenService, JWTTokenService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = fewBoxConfig.JWT.Issuer,
                    ValidAudience = fewBoxConfig.JWT.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(fewBoxConfig.JWT.Key))
                };
            });
            // Used for ApiVersion
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true; // Show versions in response.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.DefaultApiVersion = new ApiVersion(1, 0); // new ApiVersion(1, 0, "alpha");
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
            if(fewBoxConfig.Orm.InternalConnectionType == OrmConnectionType.Tenant)
            {
                services.AddScoped<IOrmConfiguration, AppSettingTenantOrmConfiguration>();
            }
            else{
                services.AddSingleton<IOrmConfiguration, AppSettingOrmConfiguration>();
            }
        }
    }
}