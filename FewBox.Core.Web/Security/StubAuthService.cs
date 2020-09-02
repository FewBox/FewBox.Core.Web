using System.Collections.Generic;

namespace FewBox.Core.Web.Security
{
    public class StubeAuthService : IAuthService
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