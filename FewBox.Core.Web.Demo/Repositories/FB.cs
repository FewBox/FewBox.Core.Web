using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Web.Demo.Repositories
{
    public class FB : Entity<Guid>
    {
        public string Name { get; set; }
    }
}