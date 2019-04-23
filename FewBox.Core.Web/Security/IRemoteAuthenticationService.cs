using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Security
{
    public interface IRemoteAuthenticationService
    {
        bool IsValid(string username, string password, out IList<string> roles);
        IList<string> FindRolesByControllerAndAction(string controller, string action, IHeaderDictionary headers);
    }
}