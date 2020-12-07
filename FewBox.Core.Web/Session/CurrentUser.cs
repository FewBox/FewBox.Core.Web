using System.Linq;
using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Http;
using FewBox.Core.Utility.Converter;

namespace FewBox.Core.Web.Token
{
    public class CurrentUser<T> : ICurrentUser<T>
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }
        public T GetId()
        {
            T id = default(T);
            if (this.HttpContextAccessor.HttpContext != null)
            {
                var claim = this.HttpContextAccessor.HttpContext.User.Claims.Where(p => p.Type == TokenClaims.Id).FirstOrDefault();
                if (claim != null)
                {
                    id = TypeUtility.Converte<T>(claim.Value);
                }
            }
            return id;
        }
    }
}