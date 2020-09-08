using System.Collections.Generic;
using System.Linq;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Security
{
    public class RemoteAuthService : IAuthService
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private FewBoxConfig FewBoxConfig { get; set; }

        public RemoteAuthService(IHttpContextAccessor httpContextAccessor, FewBoxConfig fewboxConfig)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.FewBoxConfig = fewboxConfig;
        }

        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            string url = $"{this.FewBoxConfig.SecurityEndpoint.Protocol}://{this.FewBoxConfig.SecurityEndpoint.Host}:{this.FewBoxConfig.SecurityEndpoint.Port}/api/auth/{service}/{controller}/{action}";
            return this.VerifyRoles(url, roles);
        }

        public bool DoesUserHavePermission(string service, AuthCodeType authCodeType, string code, IList<string> roles)
        {
            string url = $"{this.FewBoxConfig.SecurityEndpoint.Protocol}://{this.FewBoxConfig.SecurityEndpoint.Host}:{this.FewBoxConfig.SecurityEndpoint.Port}/api/auth/{service}/{authCodeType.ToString().ToLower()}/{code}";
            return this.VerifyRoles(url, roles);
        }

        private bool VerifyRoles(string url, IList<string> roles)
        {
            var headers = new List<Header>();
            PayloadResponseDto<IList<string>> response = null;
            if (this.HttpContextAccessor.HttpContext.Request.Headers.Keys.Contains("Authorization"))
            {
                response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>(url,
                    this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"],
                    headers);

            }
            else
            {
                response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>(url,
                    headers);
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