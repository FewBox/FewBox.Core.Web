using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public class FewBoxAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        const string ROLEPOLICY_PREFIX = "JWTRole_";
        const string PAYLOADPOLICY_PREFIX = "JWTPayload_";
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            var result = new AuthorizationPolicyBuilder();
            result = result.RequireAssertion(context =>
            {
                return true;
            });
            return Task.FromResult(result.Build());
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(ROLEPOLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
            Enum.TryParse<FewBoxPolicyType>(policyName.Substring(ROLEPOLICY_PREFIX.Length), out FewBoxPolicyType rolePolicyType))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new RoleRequirement(rolePolicyType));
                return Task.FromResult(policy.Build());
            }
            else if (policyName.StartsWith(PAYLOADPOLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
            Enum.TryParse<FewBoxPolicyType>(policyName.Substring(PAYLOADPOLICY_PREFIX.Length), out FewBoxPolicyType payloadPolicyType))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PayloadRequirement(payloadPolicyType));
                return Task.FromResult(policy.Build());
            }
            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}