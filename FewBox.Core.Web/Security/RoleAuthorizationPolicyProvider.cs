using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public class RoleAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "JWTRole_";
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)&&
            Enum.TryParse<RolePolicyType>(policyName.Substring(POLICY_PREFIX.Length), out RolePolicyType rolePolicyType))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new RoleRequirement(rolePolicyType));
                return Task.FromResult(policy.Build());
            }
            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}