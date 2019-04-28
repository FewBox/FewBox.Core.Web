using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.App.Demo.Repositories
{
    public class App : Entity<Guid>
    {
        public string Name { get; set; }
    }
}