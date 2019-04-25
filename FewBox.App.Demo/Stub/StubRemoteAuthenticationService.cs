using System.Collections.Generic;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.App.Demo.Stub
{
    public class StubRemoteAuthenticationService : IRemoteAuthenticationService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action, IHeaderDictionary headers)
        {
            return new List<string> { "Admin" };
        }
        public bool IsValid(string username, string password, string userType, out IList<string> roles)
        {
            roles = new List<string> { };
            return true;
        }
    }
}