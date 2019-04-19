using System;

namespace FewBox.Core.Web.Security
{
    static class ExpireTimes
    {
        public static TimeSpan Token = TimeSpan.FromMinutes(5);
    }
}