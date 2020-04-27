using System;
using System.Threading.Tasks;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Security
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private SecurityConfig SecurityConfig { get; set; }
        private IAuthService AuthService { get; set; }
        private ITokenService TokenService { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private ILogger<RoleHandler> Logger { get; set; }

        public RoleHandler(SecurityConfig securityConfig, IAuthService authService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ILogger<RoleHandler> logger)
        {
            this.SecurityConfig = securityConfig;
            this.AuthService = authService;
            this.TokenService = tokenService;
            this.HttpContextAccessor = httpContextAccessor;
            this.Logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            var routeData = this.HttpContextAccessor.HttpContext.GetRouteData();
            string method = this.HttpContextAccessor.HttpContext.Request.Method;
            string authorization = this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string controller = routeData.Values["controller"].ToString();
            string action = routeData.Values["action"].ToString();

            bool doesUserHavePermission = false;
            if (!String.IsNullOrEmpty(authorization))
            {
                string token = authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
                var userProfile = this.TokenService.GetUserProfileByToken(token);
                if (requirement != null)
                {
                    if (requirement.RolePolicyType == RolePolicyType.ControllerAction ||
                    requirement.RolePolicyType == RolePolicyType.ControllerAction)
                    {
                        doesUserHavePermission = this.AuthService.DoesUserHavePermission(this.SecurityConfig.Name, controller, action, userProfile.Roles);
                    }
                    else if (requirement.RolePolicyType == RolePolicyType.Method)
                    {
                        doesUserHavePermission = this.AuthService.DoesUserHavePermission(method, userProfile.Roles);
                    }
                    using (this.Logger.BeginScope($"Controller: {controller} Action: {action} Method: {method}"))
                    {
                        foreach (var header in this.HttpContextAccessor.HttpContext.Request.Headers)
                        {
                            this.Logger.LogTrace($"Header: {header.Key} - {header.Value}");
                        }
                        foreach (var claim in context.User.Claims)
                        {
                            this.Logger.LogTrace($"Claim: {claim.Type}-{claim.Value}");
                        }
                    }
                }
            }
            if (doesUserHavePermission)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}