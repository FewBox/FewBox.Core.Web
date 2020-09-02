using System;
using System.Reflection;
using System.Threading.Tasks;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Security
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private IAuthService AuthService { get; set; }
        private ITokenService TokenService { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private ILogger<RoleHandler> Logger { get; set; }

        public RoleHandler(IAuthService authService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ILogger<RoleHandler> logger)
        {
            this.AuthService = authService;
            this.TokenService = tokenService;
            this.HttpContextAccessor = httpContextAccessor;
            this.Logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            string verb = this.HttpContextAccessor.HttpContext.Request.Method;
            string authorization = this.HttpContextAccessor.HttpContext.Request.Query["access_token"].Count > 0 ?
            this.HttpContextAccessor.HttpContext.Request.Query["access_token"] : this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"];

            bool doesUserHavePermission = false;
            if (verb == HttpMethods.Options)
            {
                doesUserHavePermission = true;
            }
            else
            {
                if (!String.IsNullOrEmpty(authorization))
                {
                    string token = authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
                    var userProfile = this.TokenService.GetUserProfileByToken(token);
                    if (requirement != null)
                    {
                        string serviceName = Assembly.GetEntryAssembly().GetName().Name;
                        if (requirement.RolePolicyType == RolePolicyType.ControllerAction)
                        {
                            var routeData = this.HttpContextAccessor.HttpContext.GetRouteData();
                            string controller = routeData.Values["controller"] != null ? routeData.Values["controller"].ToString() : null;
                            string action = routeData.Values["action"] != null ? routeData.Values["action"].ToString() : null;
                            doesUserHavePermission = this.AuthService.DoesUserHavePermission(serviceName, controller, action, userProfile.Roles);
                            using (this.Logger.BeginScope($"Controller: {controller} Action: {action} Method: {verb}"))
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
                        else if (requirement.RolePolicyType == RolePolicyType.Verb)
                        {
                            doesUserHavePermission = this.AuthService.DoesUserHavePermission(serviceName, AuthCodeType.Verb, verb, userProfile.Roles);
                        }
                        else if (requirement.RolePolicyType == RolePolicyType.Hub)
                        {
                            string hub = this.HttpContextAccessor.HttpContext.Request.Path.Value.Split("/")[1];
                            doesUserHavePermission = this.AuthService.DoesUserHavePermission(serviceName, AuthCodeType.Hub, hub, userProfile.Roles);
                        }
                    }
                }
            }
            if (doesUserHavePermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                Console.WriteLine($"[False]{this.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()}###{verb}###{authorization}");
            }
            return Task.CompletedTask;
        }
    }
}