using System;
using System.Text;
using AutoMapper;
using Dapper;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Utility.Formatter;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Demo.Hubs;
using FewBox.Core.Web.Demo.Repositories;
using FewBox.Core.Web.Demo.Stub;
using FewBox.Core.Web.Error;
using FewBox.Core.Web.Filter;
using FewBox.Core.Web.Notification;
using FewBox.Core.Web.Orm;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Sentry;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Sentry.Extensibility;

namespace FewBox.Core.Web.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            this.Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(); // AddNewtonsoftJson is for JsonPatchDocument.
            SqlMapper.AddTypeHandler(new SQLiteGuidTypeHandler()); // Note: SQLite
            RestfulUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
            RestfulUtility.IsLogging = true; // Todo: Need to remove.
            JsonUtility.IsCamelCase = true; // Is camel case.
            JsonUtility.IsNullIgnore = true; // Ignore null json property.
            HttpUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
            HttpUtility.IsEnsureSuccessStatusCode = false; // Whether ensure sucess status code.
            services.AddRouting(options => options.LowercaseUrls = true); // Lowercase the urls.
            services.AddMvc(options =>
            {
                options.Filters.Add<TransactionAsyncFilter>(); // Add DB transaction.
                options.Filters.Add<TraceAsyncFilter>(); // Add biz trace log.
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins("https://fewbox.com").AllowAnyMethod().AllowAnyHeader();
                        });
                    options.AddPolicy("all",
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                        });
                }); // Cors.
            services.AddAutoMapper(typeof(Startup)); // Auto Mapper.
            services.AddMemoryCache(); // Memory cache.
            services.AddSingleton<IExceptionProcessorService, ExceptionProcessorService>(); // Catch Exception.
            // Used for Config.
            // Used for [Authorize(Policy="JWTRole_ControllerAction")].
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig); // JWT.
            var securityConfig = this.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
            services.AddSingleton(securityConfig); // Auth Service.
            var notificationConfig = this.Configuration.GetSection("NotificationConfig").Get<NotificationConfig>();
            services.AddSingleton(notificationConfig); // Notification Service.
            var healthyConfig = this.Configuration.GetSection("HealthyConfig").Get<HealthyConfig>();
            services.AddSingleton(healthyConfig); // Healthz.
            // Used for RBAC AOP.
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            //services.AddScoped<IAuthService, RemoteAuthService>();
            services.AddScoped<IAuthService, StubAuthService>();
            // Used for ORM.
            services.AddScoped<IOrmConfiguration, AppSettingOrmConfiguration>();
            // services.AddScoped<IOrmSession, MySqlSession>(); // Note: MySql
            services.AddScoped<IOrmSession, SQLiteSession>(); // Note: SQLite
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            // Used for Application.
            services.AddScoped<IFewBoxRepository, FewBoxRepository>();
            // Used for Exception&Log AOP.
            // services.AddScoped<INotificationHandler, ConsoleNotificationHandler>();
            services.AddScoped<INotificationHandler, ServiceNotificationHandler>();
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
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
            // Used for Swagger Open Api Document.
            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "FewBox demo Api";
                    document.Info.Description = "FewBox shipping, for more information please visit the 'https://fewbox.com'";
                    document.Info.TermsOfService = "https://fewbox.com/terms";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "FewBox",
                        Email = "support@fewbox.com",
                        Url = "https://fewbox.com/support"
                    };
                    document.Info.License = new OpenApiLicense
                    {
                        Name = "Use under license",
                        Url = "https://raw.githubusercontent.com/FewBox/FewBox.Service.Shipping/master/LICENSE"
                    };
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                config.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Bearer [Token]",
                        In = OpenApiSecurityApiKeyLocation.Header
                    })
                );
            });
            // Used for Sentry
            services.AddTransient<ISentryEventProcessor, SentryEventProcessor>();
            services.AddSingleton<ISentryEventExceptionProcessor, SentryEventExceptionProcessor>();
            //  Used for SignalR
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseStaticFiles();
            app.UseCors("all");

            if (env.IsDevelopment())
            {
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            if (env.IsStaging())
            {
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            if (env.IsProduction())
            {
                app.UseReDoc();
                app.UseHsts();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("notificationHub");
                endpoints.MapControllers();
            });
        }
    }
}
