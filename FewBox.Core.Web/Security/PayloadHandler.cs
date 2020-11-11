using System;
using System.Linq;
using System.Threading.Tasks;
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

        public PayloadHandler(IAuthService authService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ILogger<PayloadHandler> logger)
        {
            this.TokenService = tokenService;
            this.HttpContextAccessor = httpContextAccessor;
            this.Logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PayloadRequirement requirement)
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
                        var routeData = this.HttpContextAccessor.HttpContext.GetRouteData();
                        string controller = routeData.Values["controller"] != null ? routeData.Values["controller"].ToString() : null;
                        string action = routeData.Values["action"] != null ? routeData.Values["action"].ToString() : null;
                        doesUserHavePermission = userProfile.Apis != null ? userProfile.Apis.Count(a => a.ToLower() == $"{controller}/{action}".ToLower()) > 0 : false;
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