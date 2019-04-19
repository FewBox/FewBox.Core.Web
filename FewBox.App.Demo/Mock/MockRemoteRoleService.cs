using System.Collections.Generic;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.App.Demo.Mock
{
    public class MockRemoteRoleService : IRemoteRoleService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
        {
            return new List<string> { "Admin" };
        }

        public IList<string> FindRolesByControllerAndAction(string controller, string action, IHeaderDictionary headers)
        {
            return new List<string> { "Admin" };
        }
    }
}