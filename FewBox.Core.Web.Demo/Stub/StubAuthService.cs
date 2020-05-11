using System.Collections.Generic;
using FewBox.Core.Web.Security;

namespace FewBox.Core.Web.Demo.Stub
{
    public class StubAuthService : IAuthService
    {
        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            return true;
        }

        public bool DoesUserHavePermission(string service, AuthCodeType authCodeType, string code, IList<string> roles)
        {
            return true;
        }
    }
}