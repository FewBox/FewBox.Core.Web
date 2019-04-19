using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public class RemoteRoleRequirement : IAuthorizationRequirement
    {
        public RemoteProcedureCallType RemoteProcedureCallType { get; set; }
        public RemoteRoleRequirement(RemoteProcedureCallType remoteProcedureCallType)
        {
            this.RemoteProcedureCallType = remoteProcedureCallType;
        }
    }
}