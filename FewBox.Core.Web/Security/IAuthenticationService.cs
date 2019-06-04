using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Security
{
    public interface IAuthenticationService
    {
        bool IsValid(string username, string password, string userType, out object userId,out IList<string> roles);
        IList<string> FindRolesByServiceAndControllerAndAction(string service, string controller, string action);
        IList<string> FindRolesByMethod(string method);
    }
}