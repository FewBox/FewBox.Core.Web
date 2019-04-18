using System.Collections.Generic;

namespace FewBox.Core.Web.Security
{
    public interface IRemoteRoleService
    {
        IList<string> FindRolesByControllerAndAction(string controller, string action);
    }
}