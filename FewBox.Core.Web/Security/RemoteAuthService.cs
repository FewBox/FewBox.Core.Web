using System.Collections.Generic;
using System.Linq;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Security
{
    public class RemoteAuthService : IAuthService
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private SecurityConfig SecurityConfig { get; set; }
        private ITryCatchService TryCatchService { get; set; }

        public RemoteAuthService(IHttpContextAccessor httpContextAccessor, SecurityConfig securityConfig, ITryCatchService tryCatchService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.SecurityConfig = securityConfig;
            this.TryCatchService = tryCatchService;
        }

        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            string url = $"{this.SecurityConfig.Protocol}://{this.SecurityConfig.Host}:{this.SecurityConfig.Port}/api/auth/{service}/{controller}/{action}";
            return this.VerifyRoles(url, roles);
        }

        public bool DoesUserHavePermission(string service, AuthCodeType authCodeType, string code, IList<string> roles)
        {
            string url = $"{this.SecurityConfig.Protocol}://{this.SecurityConfig.Host}:{this.SecurityConfig.Port}/api/auth/{service}/{authCodeType.ToString().ToLower()}/{code}";
            return this.VerifyRoles(url, roles);
        }

        private bool VerifyRoles(string url, IList<string> roles)
        {
            var headers = new List<Header>();
            PayloadResponseDto<IList<string>> response = null;
            if (this.HttpContextAccessor.HttpContext.Request.Headers.Keys.Contains("Authorization"))
            {
                this.TryCatchService.TryCatch(() =>
                {
                    response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>(url,
                    this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"],
                    headers);
                });

            }
            else
            {
                this.TryCatchService.TryCatch(() =>
                {
                    response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>(url,
                    headers);
                });
            }
            if (response != null)
            {
                return response.Payload.Intersect(roles).Count() > 0;
            }
            else
            {
                return false;
            }
        }
    }
}