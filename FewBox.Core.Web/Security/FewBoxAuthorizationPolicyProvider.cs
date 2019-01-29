using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Fewbox.Core.Web.Security
{
    public class FewBoxAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            throw new System.NotImplementedException();
        }
    }
}