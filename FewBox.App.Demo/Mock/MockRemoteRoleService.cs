using System.Collections.Generic;
using FewBox.Core.Web.Security;

namespace FewBox.App.Demo.Mock
{
    public class MockRemoteRoleService : IRemoteRoleService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
        {
            return new List<string> { "Admin" };
        }
    }
}