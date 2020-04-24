using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Web.Demo.Repositories
{
    public interface IFewBoxRepository : IBaseRepository<FewBox, Guid>
    {
    }
}