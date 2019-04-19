﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FewBox.Core.Web.Security
{
    public class RemoteRoleHandler : AuthorizationHandler<RemoteRoleRequirement>
    {
        private IRemoteAuthenticationService RemoteAuthenticationService { get; set; }
        private IActionContextAccessor ActionContextAccessor { get; set; }
        
        public RemoteRoleHandler(IRemoteAuthenticationService remoteAuthenticationService, IActionContextAccessor actionContextAccessor)
        {
            this.RemoteAuthenticationService = remoteAuthenticationService;
            this.ActionContextAccessor = actionContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RemoteRoleRequirement requirement)
        {
            string controller = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["controller"];
            string action = this.ActionContextAccessor.ActionContext.ActionDescriptor.RouteValues["action"];
            IList<string> roles;
            if(requirement.RemoteProcedureCallType == RemoteProcedureCallType.WithHeader)
            {
                roles = this.RemoteAuthenticationService.FindRolesByControllerAndAction(controller, action, this.ActionContextAccessor.ActionContext.HttpContext.Request.Headers);
            }
            else
            {
                roles = this.RemoteAuthenticationService.FindRolesByControllerAndAction(controller, action);
            }
            foreach(string role in roles)
            {
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