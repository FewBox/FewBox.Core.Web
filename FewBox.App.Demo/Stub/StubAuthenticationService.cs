using System.Collections.Generic;
using FewBox.Core.Web.Security;

namespace FewBox.App.Demo.Stub
{
    public class StubAuthenticationService : IAuthService
    {
        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            return true;
        }

        public bool DoesUserHavePermission(string method, IList<string> roles)
        {
            return true;
        }
    }
}