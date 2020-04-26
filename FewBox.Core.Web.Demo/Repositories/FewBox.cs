using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Web.Demo.Repositories
{
    public class FewBox : Entity
    {
        public string Name { get; set; }
    }
}