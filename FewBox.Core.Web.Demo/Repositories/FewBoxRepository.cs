using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Web.Demo.Repositories
{
    public class FewBoxRepository : BaseRepository<FewBox, Guid>, IFewBoxRepository
    {
        public FewBoxRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("fb", ormSession, currentUser)
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