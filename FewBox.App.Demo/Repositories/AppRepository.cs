using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.App.Demo.Repositories
{
    public class AppRepository : BaseRepository<App, Guid>, IAppRepository
    {
        public AppRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("app", ormSession, currentUser)
        {
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}