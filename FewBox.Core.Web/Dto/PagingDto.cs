using System.Collections.Generic;

namespace FewBox.Core.Web.Dto
{
    public class PagingDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PagingCount { get; set; }
    }
}