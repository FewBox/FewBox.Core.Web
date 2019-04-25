using Microsoft.AspNetCore.Authorization;

namespace FewBox.Core.Web.Security
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RolePolicyType RolePolicyType { get; set; }
        public RoleRequirement(RolePolicyType rolePolicyType)
        {
            this.RolePolicyType = rolePolicyType;
        }
    }
}