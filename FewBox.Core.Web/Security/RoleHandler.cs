﻿using System;
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
            string method = this.ActionContextAccessor.ActionContext.HttpContext.Request.Method;
            string controller = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["controller"];
            string action = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["action"];
            IList<string> roles = null;
            if(requirement != null)
            {
                if(requirement.RolePolicyType == RolePolicyType.ControllerAction||
                requirement.RolePolicyType == RolePolicyType.ControllerActionWithLog)
                {
                    roles = this.AuthenticationService.FindRolesByControllerAndAction(controller, action);
                }
                else if(requirement.RolePolicyType == RolePolicyType.Method||
                requirement.RolePolicyType == RolePolicyType.Method)
                {
                    roles = this.AuthenticationService.FindRolesByMethod(method);
                }
                if(requirement.RolePolicyType == RolePolicyType.ControllerActionWithLog||
                requirement.RolePolicyType == RolePolicyType.MethodWithLog)
                {
                    Console.WriteLine($"Controller: {controller}");
                    Console.WriteLine($"Action: {action}");
                    Console.WriteLine($"Method: {method}");
                    foreach(var header in this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers)
                    {
                        Console.WriteLine($"Header: {header.Key} - {header.Value}");
                    }
                }
            }
            if(roles != null)
            {
                foreach(string role in roles)
                {
                    if(context.User.IsInRole(role))
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}