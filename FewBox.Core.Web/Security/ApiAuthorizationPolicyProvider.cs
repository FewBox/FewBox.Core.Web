using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Fewbox.Core.Web.Security
{
    public class ApiAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            /*if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new ApiRequirement());
                return Task.FromResult(policy.Build());
            } */

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}