using System;
using System.Linq;
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
    public class PayloadHandler : AuthorizationHandler<PayloadRequirement>
    {
        private ITokenService TokenService { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private ILogger<PayloadHandler> Logger { get; set; }
        private FewBoxConfig FewBoxConfig { get; set; }

        public PayloadHandler(IAuthService authService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ILogger<PayloadHandler> logger, FewBoxConfig fewBoxConfig)
        {
            this.TokenService = tokenService;
            this.HttpContextAccessor = httpContextAccessor;
            this.Logger = logger;
            this.FewBoxConfig = fewBoxConfig;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PayloadRequirement requirement)
        {
            string verb = this.HttpContextAccessor.HttpContext.Request.Method;
            string authorization = this.HttpContextAccessor.HttpContext.Request.Query["access_token"].Count > 0 ?
            this.HttpContextAccessor.HttpContext.Request.Query["access_token"] : this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = String.IsNullOrEmpty(authorization) ? null : authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
            if (!String.IsNullOrEmpty(token) && this.TokenService.ValidateToken(token, this.FewBoxConfig.JWT.Key, this.FewBoxConfig.JWT.Issuer, this.FewBoxConfig.JWT.Audience))
            {
                bool doesUserHavePermission = false;
                string controller = "Unknown";
                string action = "Unknown";
                if (verb == HttpMethods.Options)
                {
                    doesUserHavePermission = true;
                }
                else
                {
                    var userProfile = this.TokenService.GetUserProfileByToken(token);
                    if (requirement != null)
                    {
                        string service = Assembly.GetEntryAssembly().GetName().Name;
                        var routeData = this.HttpContextAccessor.HttpContext.GetRouteData();
                        controller = routeData.Values["controller"] != null ? routeData.Values["controller"].ToString() : null;
                        action = routeData.Values["action"] != null ? routeData.Values["action"].ToString() : null;
                        doesUserHavePermission = userProfile.Apis != null ? userProfile.Apis.Count(a => a.ToLower() == $"{service}/{controller}/{action}".ToLower()) > 0 : false;
                    }
                    else
                    {
                        this.Logger.LogError("PayloadRequirement is null!");
                    }
                }
                if (doesUserHavePermission)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    this.Logger.LogError($"{controller} {action} {this.HttpContextAccessor.HttpContext.Request.GetDisplayUrl()}###{verb}###{token}");
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