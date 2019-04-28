using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.App.Demo.Repositories
{
    public interface IAppRepository : IBaseRepository<App, Guid>
    {
    }
}