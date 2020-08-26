using System;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FewBox.Core.Web.Orm
{
    public class AppSettingTenantOrmConfiguration : IOrmConfiguration
    {
        private string ConnectionString { get; set; }

        public AppSettingTenantOrmConfiguration(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            string authorization = httpContextAccessor.HttpContext.Request.Query["access_token"].Count > 0 ?
            httpContextAccessor.HttpContext.Request.Query["access_token"] : httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!String.IsNullOrEmpty(authorization))
            {
                string token = authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
                UserProfile userProfile = tokenService.GetUserProfileByToken(token);
                string connectionString = configuration.GetConnectionString("TenantConnection").Replace("[Tenant]", userProfile.Tenant);
                this.ConnectionString = connectionString;
            }
            else
            {
                string defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
                if (String.IsNullOrEmpty(defaultConnectionString))
                {
                    this.ConnectionString = "";
                }
                else
                {
                    this.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                }
            }
        }

        public string GetConnectionString()
        {
            return this.ConnectionString;
        }
    }
}
