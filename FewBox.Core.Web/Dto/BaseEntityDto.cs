using System;

namespace FewBox.Core.Web.Dto
{
    public class BaseEntityDto<TID>
    {
        public TID Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public TID CreatedBy { get; set; }
        public TID ModifiedBy { get; set; }
    }
}