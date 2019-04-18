using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public class RemoteRoleRequirement : IAuthorizationRequirement
    {
        public RemoteType RemoteType { get; set; }
        public RemoteRoleRequirement(RemoteType remoteType)
        {
            this.RemoteType = remoteType;
        }
    }
}