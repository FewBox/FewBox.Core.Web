using System;
using System.Collections.Generic;
using System.Linq;
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
using Swashbuckle.AspNetCore.Swagger;
using FewBox.App.Demo.Stub;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FewBox.Core.Web.Config;
using FewBox.App.Demo.Repositories;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Orm;
using FewBox.Core.Web.Filter;
using Dapper;
using AutoMapper;

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
            SqlMapper.AddTypeHandler(new SQLiteGuidTypeHandler());
            services.AddAutoMapper();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options => {
                options.Filters.Add<ExceptionAsyncFilter>();
                options.Filters.Add<TransactionAsyncFilter>();
                options.Filters.Add<TraceAsyncFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig);
            var securityConfig = this.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
            services.AddSingleton(securityConfig);
            services.AddScoped<ITokenService, JWTToken>();
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            services.AddScoped<IAuthenticationService, StubAuthenticationService>();
            services.AddScoped<IOrmConfiguration, AppSettingOrmConfiguration>();
            // services.AddScoped<IOrmSession, MySqlSession>();
            services.AddScoped<IOrmSession, SQLiteSession>();
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            services.AddScoped<IAppRepository, AppRepository>();
            services.AddScoped<IExceptionHandler, ConsoleExceptionHandler>();
            services.AddScoped<ITraceHandler, ConsoleTraceHandler>();
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "FewBox API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = @"Please enter JWT with Bearer into field. Example: 'Bearer {token}'", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
                // c.SwaggerDoc("v2", new Info { Title = "FewBox API", Version = "v2" });
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseStaticFiles();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FewBox API V1");
                // c.SwaggerEndpoint("/swagger/v2/swagger.json", "FewBox API V2");
                c.RoutePrefix = String.Empty;
            });
        }
    }
}
