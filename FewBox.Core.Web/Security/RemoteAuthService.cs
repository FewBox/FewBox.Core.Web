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
        private SecurityConfig SecurityConfig { get; set; }

        public RemoteAuthService(IHttpContextAccessor httpContextAccessor, SecurityConfig securityConfig)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.SecurityConfig = securityConfig;
        }

        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            var headers = new List<Header>();
            foreach (var header in this.HttpContextAccessor.HttpContext.Request.Headers)
            {
                headers.Add(new Header { Key = header.Key, Value = header.Value });
            }
            var respose = RestfulUtility.Get<PayloadResponseDto<IList<string>>>($"{this.SecurityConfig.Protocol}://{this.SecurityConfig.Host}:{this.SecurityConfig.Port}/api/auth/{service}/{controller}/{action}", headers);
            return respose.Payload.Intersect(roles).Count() > 0;

        }

        public bool DoesUserHavePermission(string method, IList<string> roles)
        {
            throw new System.NotImplementedException();
        }
    }
}