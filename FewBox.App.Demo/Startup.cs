using System;
using System.Text;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FewBox.Core.Web.Config;
using FewBox.App.Demo.Repositories;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Orm;
using FewBox.Core.Web.Filter;
using Dapper;
using AutoMapper;
using Morcatko.AspNetCore.JsonMergePatch;
using NSwag;
using FewBox.Core.Web.Error;
using FewBox.Core.Web.Log;
using FewBox.Core.Web.Notification;
using NSwag.Generation.Processors.Security;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FewBox.App.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITokenService, JWTToken>();
            SqlMapper.AddTypeHandler(new SQLiteGuidTypeHandler());
            services.AddAutoMapper(typeof(Startup));
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionAsyncFilter>();
                options.Filters.Add<TransactionAsyncFilter>();
                options.Filters.Add<TraceAsyncFilter>();
            }).AddJsonMergePatch().SetCompatibilityVersion(CompatibilityVersion.Latest);
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig);
            var securityConfig = this.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
            services.AddSingleton(securityConfig);
            var logConfig = this.Configuration.GetSection("LogConfig").Get<LogConfig>();
            services.AddSingleton(logConfig);
            var notificationConfig = this.Configuration.GetSection("NotificationConfig").Get<NotificationConfig>();
            services.AddSingleton(notificationConfig);
            var healthyConfig = this.Configuration.GetSection("HealthyConfig").Get<HealthyConfig>();
            services.AddSingleton(healthyConfig);
            services.AddScoped<ITokenService, JWTToken>();
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            // services.AddScoped<IAuthService, StubAuthenticationService>();
            services.AddScoped<IAuthService, RemoteAuthService>();
            services.AddScoped<IOrmConfiguration, AppSettingOrmConfiguration>();
            // services.AddScoped<IOrmSession, MySqlSession>();
            services.AddScoped<IOrmSession, SQLiteSession>();
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            services.AddScoped<IFBRepository, FBRepository>();
            services.AddScoped<ILogHandler, ConsoleLogHandler>();
            services.AddScoped<INotificationHandler, ServiceNotificationHandler>();
            services.AddScoped<ITryCatchService, TryCatchService>();
            services.AddSingleton<IExceptionProcessorService, ExceptionProcessorService>();
            //services.AddScoped<IExceptionHandler, ServiceExceptionHandler>();
            //services.AddScoped<ITraceHandler, ServiceTraceHandler>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseOpenApi();
            app.UseStaticFiles();
            if (env.EnvironmentName == "development" || env.EnvironmentName == "staging")
            {
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseReDoc();
            }
            app.UseMvc();
        }
    }
}
