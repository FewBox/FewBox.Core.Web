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
        private FewBoxConfig FewBoxConfig { get; set; }

        public RoleHandler(IAuthService authService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ILogger<RoleHandler> logger, FewBoxConfig fewBoxConfig)
        {
            this.AuthService = authService;
            this.TokenService = tokenService;
            this.HttpContextAccessor = httpContextAccessor;
            this.Logger = logger;
            this.FewBoxConfig = fewBoxConfig;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            string verb = this.HttpContextAccessor.HttpContext.Request.Method;
            string authorization = this.HttpContextAccessor.HttpContext.Request.Query["access_token"].Count > 0 ?
            this.HttpContextAccessor.HttpContext.Request.Query["access_token"] : this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = String.IsNullOrEmpty(authorization) ? null : authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
            string service = Assembly.GetEntryAssembly().GetName().Name;
            var routeData = this.HttpContextAccessor.HttpContext.GetRouteData();
            string controller = routeData.Values["controller"] != null ? routeData.Values["controller"].ToString() : null;
            string action = routeData.Values["action"] != null ? routeData.Values["action"].ToString() : null;
            if (!String.IsNullOrEmpty(token) && this.TokenService.ValidateToken(token, this.FewBoxConfig.JWT.Key, this.FewBoxConfig.JWT.Issuer, this.FewBoxConfig.JWT.Audience))
            {
                bool doesUserHavePermission = false;
                if (verb == HttpMethods.Options)
                {
                    doesUserHavePermission = true;
                }
                else
                {
                    var userProfile = this.TokenService.GetUserProfileByToken(token);
                    if (requirement != null)
                    {
                        if (requirement.FewBoxPolicyType == FewBoxPolicyType.ControllerAction)
                        {
                            doesUserHavePermission = this.AuthService.DoesUserHavePermission(service, controller, action, userProfile.Roles);
                        }
                        else if (requirement.FewBoxPolicyType == FewBoxPolicyType.Verb)
                        {
                            doesUserHavePermission = this.AuthService.DoesUserHavePermission(service, AuthCodeType.Verb, verb, userProfile.Roles);
                        }
                        else if (requirement.FewBoxPolicyType == FewBoxPolicyType.Hub)
                        {
                            string hub = this.HttpContextAccessor.HttpContext.Request.Path.Value.Split("/")[1];
                            doesUserHavePermission = this.AuthService.DoesUserHavePermission(service, AuthCodeType.Hub, hub, userProfile.Roles);
                        }
                    }
                    else
                    {
                        this.Logger.LogError("RoleRequirement is null!");
                    }
                }
                if (doesUserHavePermission)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    this.Logger.LogError($"[FewBox JWTPayload] {service} {controller} {action} {verb} {token}");
                    this.HttpContextAccessor.HttpContext.Response.StatusCode = 403;
                    context.Fail();
                    /*using (this.Logger.BeginScope($"[FewBox] Controller: {controller} Action: {action} Method: {verb}"))
                    {
                        this.Logger.LogWarning($"{controller} {action} {this.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()}###{verb}###{authorization}");
                        foreach (var header in this.HttpContextAccessor.HttpContext.Request.Headers)
                        {
                            this.Logger.LogDebug($"Header: {header.Key} - {header.Value}");
                        }
                        foreach (var claim in context.User.Claims)
                        {
                            this.Logger.LogDebug($"Claim: {claim.Type}-{claim.Value}");
                        }
                    }*/
                }
            }
            else
            {
                context.Fail();
                this.Logger.LogWarning($"[FewBox] Token Invalid {this.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()}###{verb}###{authorization}");
            }
            return Task.CompletedTask;
        }
    }
}