using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Web.Demo.Repositories
{
    public interface IFBRepository : IBaseRepository<FB, Guid>
    {
    }
}