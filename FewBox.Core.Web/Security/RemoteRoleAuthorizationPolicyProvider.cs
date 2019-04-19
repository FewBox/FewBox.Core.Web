using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public class RemoteRoleAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "RemoteRole_";
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)&&
            Enum.TryParse<RemoteProcedureCallType>(policyName.Substring(POLICY_PREFIX.Length), out RemoteProcedureCallType remoteProcedureCallType))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new RemoteRoleRequirement(remoteProcedureCallType));
                return Task.FromResult(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}