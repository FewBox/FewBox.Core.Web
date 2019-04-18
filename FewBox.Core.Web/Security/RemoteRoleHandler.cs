using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Security
{
    public class RemoteRoleHandler : AuthorizationHandler<RemoteRoleRequirement>
    {
        private IRemoteRoleService RemoteRoleService { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        
        public RemoteRoleHandler(IRemoteRoleService remoteRoleService, IHttpContextAccessor httpContextAccessor)
        {
            this.RemoteRoleService = remoteRoleService;
            this.HttpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RemoteRoleRequirement requirement)
        {
            string controller = this.HttpContextAccessor.HttpContext.Request.Host.Host;
            string action = string.Empty;
            foreach(string role in this.RemoteRoleService.FindRolesByControllerAndAction(controller, action))
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