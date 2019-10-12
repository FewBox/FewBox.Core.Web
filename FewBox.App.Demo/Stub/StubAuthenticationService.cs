using System.Collections.Generic;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.App.Demo.Stub
{
    public class StubAuthenticationService : IAuthService
    {
        public IList<string> FindRoles(string service, string controller, string action)
        {
            if(action.Contains("Get"))
            {
                return new List<string>{ "Admin", "Normal" };
            }
            else
            {
                return new List<string> { "Admin" };
            }
        }

        public IList<string> FindRoles(string method)
        {
            if(HttpMethods.IsGet(method))
            {
                return new List<string> { "Admin", "Normal" };
            }
            else
            {
                return new List<string> { "Admin" };
            }
        }
    }
}