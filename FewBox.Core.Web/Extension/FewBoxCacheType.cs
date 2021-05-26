

using System;

namespace FewBox.Core.Web.Extension
{
    [Flags]
    public enum FewBoxCacheType
    {
        None = 0,
        Memory = 1,
        Redis = 2
    }
}