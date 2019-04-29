using System;
using System.Collections.Generic;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.App.Demo.Stub
{
    public class StubAuthenticationService : IAuthenticationService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
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

        public IList<string> FindRolesByMethod(string method)
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
        public bool IsValid(string username, string password, string userType, out object userId, out IList<string> roles)
        {
            userId = Guid.NewGuid();
            if(username == "fewbox")
            {
                roles = new List<string> { "Admin" };
            }
            else
            {
                roles = new List<string> { "Normal" };
            }
            return true;
        }
    }
}