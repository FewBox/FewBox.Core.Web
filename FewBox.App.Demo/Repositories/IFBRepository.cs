using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.App.Demo.Repositories
{
    public interface IFBRepository : IBaseRepository<FB, Guid>
    {
    }
}