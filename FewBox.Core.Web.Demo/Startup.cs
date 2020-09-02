using System;
using FewBox.Core.Web.Demo.Hubs;
using FewBox.Core.Web.Demo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using NSwag.Generation.AspNetCore;
using FewBox.Core.Web.Extension;

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
        private Action<OpenApiDocument> OpenApiDocumentAction { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFewBox(FewBoxDBType.SQLite);
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins("https://fewbox.com", "https://figma.com")
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                        });
                    options.AddPolicy("dev",
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            //.AllowAnyOrigin()
                            .WithOrigins("http://localhost", "https://localhost")
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                        });
                }); // Cors.
            // Used for Application.
            services.AddScoped<IFewBoxRepository, FewBoxRepository>();
            // Used for Swagger Open Api Document.
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v1", new[] { "1-alpha", "1-beta", "1" }, "v1");
            });
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v2", new[] { "2-alpha", "2-beta", "2" }, "v2");
            });
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
            app.UseCors("dev");

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
                app.UseReDoc(c => c.DocumentPath = "/swagger/v1/swagger.json");
                app.UseReDoc(c => c.DocumentPath = "/swagger/v2/swagger.json");
                app.UseHsts();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("notificationHub");
                endpoints.MapControllers();
            });
        }

        private void InitAspNetCoreOpenApiDocumentGeneratorSettings(AspNetCoreOpenApiDocumentGeneratorSettings config, string documentName, string[] apiGroupNames, string documentVersion)
        {
            config.DocumentName = documentName;
            config.ApiGroupNames = apiGroupNames;
            config.PostProcess = document =>
            {
                this.InitDocumentInfo(document, documentVersion);
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

        private void InitDocumentInfo(OpenApiDocument document, string version)
        {
            document.Info.Version = version;
            document.Info.Title = "FewBox Demo Api";
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
                Url = "https://fewbox.com/license"
            };
        }
    }
}
