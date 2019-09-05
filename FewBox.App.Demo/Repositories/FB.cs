using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.App.Demo.Repositories
{
    public class FB : Entity<Guid>
    {
        public string Name { get; set; }
    }
}