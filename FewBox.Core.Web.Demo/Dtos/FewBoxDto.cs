using System;

namespace FewBox.Core.Web.Demo.Dtos
{
    public class FewBoxDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}