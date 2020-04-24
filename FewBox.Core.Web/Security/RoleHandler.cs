using System;
using System.Threading.Tasks;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FewBox.Core.Web.Security
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private SecurityConfig SecurityConfig { get; set; }
        private IAuthService AuthService { get; set; }
        private ITokenService TokenService { get; set; }
        private IActionContextAccessor ActionContextAccessor { get; set; }

        public RoleHandler(SecurityConfig securityConfig, IAuthService authService, ITokenService tokenService, IActionContextAccessor actionContextAccessor)
        {
            this.SecurityConfig = securityConfig;
            this.AuthService = authService;
            this.TokenService = tokenService;
            this.ActionContextAccessor = actionContextAccessor;
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
                    requirement.RolePolicyType == RolePolicyType.ControllerActionWithLog)
                    {
                        doesUserHavePermission = this.AuthService.DoesUserHavePermission(this.SecurityConfig.Name, controller, action, userProfile.Roles);
                    }
                    else if (requirement.RolePolicyType == RolePolicyType.Method ||
                    requirement.RolePolicyType == RolePolicyType.Method)
                    {
                        doesUserHavePermission = this.AuthService.DoesUserHavePermission(method, userProfile.Roles);
                    }
                    if (requirement.RolePolicyType == RolePolicyType.ControllerActionWithLog ||
                    requirement.RolePolicyType == RolePolicyType.MethodWithLog)
                    {
                        Console.WriteLine($"Controller: {controller}");
                        Console.WriteLine($"Action: {action}");
                        Console.WriteLine($"Method: {method}");
                        foreach (var header in this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers)
                        {
                            Console.WriteLine($"Header: {header.Key} - {header.Value}");
                        }
                        foreach (var claim in context.User.Claims)
                        {
                            Console.WriteLine($"Claim: {claim.Type}-{claim.Value}");
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