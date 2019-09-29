using System.Collections.Generic;

namespace FewBox.Core.Web.Security
{
    public interface IAuthService
    {
        IList<string> FindRoles(string service, string controller, string action);
        IList<string> FindRoles(string method);
    }
}