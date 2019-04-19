using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Security
{
    public interface IRemoteRoleService
    {
        IList<string> FindRolesByControllerAndAction(string controller, string action);
        IList<string> FindRolesByControllerAndAction(string controller, string action, IHeaderDictionary headers);
    }
}