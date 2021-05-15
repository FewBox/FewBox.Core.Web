using AutoMapper;
using System;
using System.Linq;
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
using Microsoft.AspNetCore.Builder;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using NSwag;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FewBox.Core.Web.Extension
{
    public static class FewBoxWebExtension
    {
        private static IConfigurationRoot Configuration { get; set; }
        private static IWebHostEnvironment WebHostEnvironment { get; set; }
        static FewBoxWebExtension()
        {
            // Init config.
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("./appsettings.json")
            .AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();
            Configuration = configuration;
        }

        public static void AddFewBox(this IServiceCollection services, IList<ApiVersionDocument> apiVersionDocuments, FewBoxDBType fewBoxDBType = FewBoxDBType.MySQL, FewBoxAuthType fewBoxAuthType = FewBoxAuthType.Payload, int responseCacheDuration = 3600)
        {
            // Support Json Patch
            // services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            });
            // Config
            var fewBoxConfig = Configuration.GetSection("FewBox").Get<FewBoxConfig>();
            services.AddSingleton(fewBoxConfig);
            // Init env.
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IWebHostEnvironment webHostEnvironment = serviceProvider.GetService<IWebHostEnvironment>();
            WebHostEnvironment = webHostEnvironment;
            // Switch env.
            if (webHostEnvironment.IsProduction())
            {
                RestfulUtility.IsCertificateNeedValidate = true; // Whether check the ceritfication.
                RestfulUtility.IsLogging = false; // Is logging request.
                HttpUtility.IsCertificateNeedValidate = true; // Whether check the ceritfication.
                services.AddScoped<IAuthService, RemoteAuthService>();
            }
            else
            {
                RestfulUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
                RestfulUtility.IsLogging = true; // Is logging request.
                HttpUtility.IsCertificateNeedValidate = false; // Whether check the ceritfication.
                services.AddScoped<IAuthService, StubeAuthService>();
            }
            // JWT
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
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = fewBoxConfig.JWT.Issuer,
                    ValidAudience = fewBoxConfig.JWT.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(fewBoxConfig.JWT.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
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
                options.DefaultApiVersion = apiVersionDocuments.Where(apiVersionDocument => apiVersionDocument.IsDefault).SingleOrDefault().ApiVersion;
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
            // Used for Swagger Open Api Document.
            foreach (var doc in GetDocs(apiVersionDocuments))
            {
                services.AddOpenApiDocument(config =>
                {
                    InitAspNetCoreOpenApiDocumentGeneratorSettings(config, doc.Version, doc.ApiGroupNames, doc.Version);
                });
            }
        }
        public static void UseFewBox(this IApplicationBuilder app, IList<ApiVersionDocument> apiVersionDocuments)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseStaticFiles();
            if (WebHostEnvironment.IsProduction() || WebHostEnvironment.IsStaging())
            {
                app.UseCors();
                foreach (var doc in GetDocs(apiVersionDocuments))
                {
                    string documentPath = $"/swagger/{doc.Version}/swagger.json";
                    app.UseReDoc(c => c.DocumentPath = documentPath);
                }
                app.UseHsts();
            }
            else if (WebHostEnvironment.IsDevelopment())
            {
                app.UseCors("dev");
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            app.UseEndpoints(endpoints =>
            {
                if (WebHostEnvironment.IsDevelopment())
                {
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                }
                else
                {
                    endpoints.MapControllers();
                }
            });
        }

        private static void InitAspNetCoreOpenApiDocumentGeneratorSettings(AspNetCoreOpenApiDocumentGeneratorSettings config, string documentName, IList<string> apiGroupNames, string documentVersion)
        {
            config.DocumentName = documentName;
            config.ApiGroupNames = apiGroupNames.ToArray();
            config.PostProcess = document =>
            {
                InitDocumentInfo(document, documentVersion);
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
        }

        private static void InitDocumentInfo(OpenApiDocument document, string version)
        {
            document.Info.Version = version;
            document.Info.Title = "FewBox Api";
            document.Info.Description = "FewBox Api for more information please visit the 'https://fewbox.com'";
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
                Url = "https://fewbox.com/license"
            };
            string service = Assembly.GetEntryAssembly().GetName().Name;
            document.Info.ExtensionData = new Dictionary<string, object> {
                {"Service", service}
            };
        }

        private static IList<Doc> GetDocs(IList<ApiVersionDocument> apiVersionDocuments)
        {
            return apiVersionDocuments.GroupBy(apiVersionDocument => apiVersionDocument.ApiVersion.MajorVersion).Select(
                group =>
                {
                    var versions = group.Select(apiVersionDocument => $"{apiVersionDocument.ApiVersion.MajorVersion}-{apiVersionDocument.ApiVersion.Status}".TrimEnd('-')).ToList();
                    return new Doc
                    {
                        Version = $"v{group.First().ApiVersion.MajorVersion}",
                        ApiGroupNames = versions
                    };
                }
                ).ToList();
        }

        private class Doc
        {
            public string Version { get; set; }
            public IList<string> ApiGroupNames { get; set; }
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}