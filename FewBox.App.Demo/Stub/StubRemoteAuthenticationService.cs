using System.Collections.Generic;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.App.Demo.Stub
{
    public class StubRemoteAuthenticationService : IRemoteAuthenticationService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
        {
            return new List<string> { "Admin" };
        }

        public IList<string> FindRolesByControllerAndAction(string controller, string action, IHeaderDictionary headers)
        {
            return new List<string> { "Admin" };
        }

        public IList<string> FindRolesByUserIdentity(object userIdentity)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValid(string username, string password, out IList<string> roles)
        {
            throw new System.NotImplementedException();
        }
    }
}