using System.Collections.Generic;

namespace FewBox.Core.Web.Security
{
    public interface IAuthService
    {
        bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles);
        bool DoesUserHavePermission(string service, AuthCodeType authCodeType, string code, IList<string> roles);
    }
}