using System;
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
        private IExceptionProcessorService ExceptionProcessorService { get; set; }

        public RemoteAuthService(IHttpContextAccessor httpContextAccessor, SecurityConfig securityConfig, IExceptionProcessorService exceptionProcessorService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.SecurityConfig = securityConfig;
            this.ExceptionProcessorService = exceptionProcessorService;
        }

        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            var headers = new List<Header>();
            PayloadResponseDto<IList<string>> response = null;
            if (this.HttpContextAccessor.HttpContext.Request.Headers.Keys.Contains("Authorization"))
            {
                this.ExceptionProcessorService.TryCatchInNotification(() =>
                {
                    response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>($"{this.SecurityConfig.Protocol}://{this.SecurityConfig.Host}:{this.SecurityConfig.Port}/api/auth/{service}/{controller}/{action}",
                    this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"],
                    headers);
                });

            }
            else
            {
                this.ExceptionProcessorService.TryCatchInNotification(() =>
                {
                    response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>($"{this.SecurityConfig.Protocol}://{this.SecurityConfig.Host}:{this.SecurityConfig.Port}/api/auth/{service}/{controller}/{action}",
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

        public bool DoesUserHavePermission(string method, IList<string> roles)
        {
            throw new System.NotImplementedException();
        }
    }
}