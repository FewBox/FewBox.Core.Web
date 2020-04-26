using System;
using System.Threading.Tasks;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Security
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private SecurityConfig SecurityConfig { get; set; }
        private IAuthService AuthService { get; set; }
        private ITokenService TokenService { get; set; }
        private IActionContextAccessor ActionContextAccessor { get; set; }
        private ILogger<RoleHandler> Logger { get; set; }

        public RoleHandler(SecurityConfig securityConfig, IAuthService authService, ITokenService tokenService, IActionContextAccessor actionContextAccessor, ILogger<RoleHandler> logger, IHttpContextAccessor accessor)
        {
            this.SecurityConfig = securityConfig;
            this.AuthService = authService;
            this.TokenService = tokenService;
            this.ActionContextAccessor = actionContextAccessor;
            this.Logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            string method = this.ActionContextAccessor.ActionContext.HttpContext.Request.Method;
            string controller = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["controller"];
            string action = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["action"];
            string authorization = this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers["Authorization"];
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
                        foreach (var header in this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers)
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