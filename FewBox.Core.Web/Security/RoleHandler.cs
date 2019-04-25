using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FewBox.Core.Web.Security
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private IAuthenticationService AuthenticationService { get; set; }
        private IActionContextAccessor ActionContextAccessor { get; set; }
        
        public RoleHandler(IAuthenticationService authenticationService, IActionContextAccessor actionContextAccessor)
        {
            this.AuthenticationService = authenticationService;
            this.ActionContextAccessor = actionContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            string controller = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["controller"];
            string action = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["action"];
            IList<string> roles;
            if(requirement!=null)
            {
                if(requirement.RolePolicyType == RolePolicyType.Header)
                {
                    Console.WriteLine($"Controller: {controller}");
                    Console.WriteLine($"Action: {action}");
                    foreach(var header in this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers)
                    {
                        Console.WriteLine($"Header: {header.Key} - {header.Value}");
                    }
                }
                else if(requirement.RolePolicyType == RolePolicyType.Cookie)
                {
                    Console.WriteLine($"Controller: {controller}");
                    Console.WriteLine($"Action: {action}");
                    foreach(var cookie in this.ActionContextAccessor.ActionContext.HttpContext.Request.Cookies)
                    {
                        Console.WriteLine($"Header: {cookie.Key} - {cookie.Value}");
                    }
                }
            }
            roles = this.AuthenticationService.FindRolesByControllerAndAction(controller, action, this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers);
            foreach(string role in roles)
            {
                // Todo: Always return false.
                if(context.User.IsInRole(role))
                {
                    context.Succeed(requirement);
                    break;
                }
            }
            return Task.CompletedTask;
        }
    }
}