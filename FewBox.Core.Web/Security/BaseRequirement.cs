using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public abstract class BaseRequirement : IAuthorizationRequirement
    {
        public FewBoxPolicyType FewBoxPolicyType { get; private set; }
        public BaseRequirement(FewBoxPolicyType fewBoxPolicyType)
        {
            this.FewBoxPolicyType = fewBoxPolicyType;
        }
    }
}