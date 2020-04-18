using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Web.Demo.Repositories
{
    public class FBRepository : BaseRepository<FB, Guid>, IFBRepository
    {
        public FBRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
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