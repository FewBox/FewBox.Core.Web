using System;
using System.Linq;
using System.Security.Claims;
using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Orm
{
    public class WebCurrentUser<TID> : ICurrentUser<TID>
    {
        private HttpContext HttpContext { get; set; }
        public WebCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContext = httpContextAccessor.HttpContext;
        }

        public TID GetId()
        {
            TID id = default(TID);
            var currentUser = this.HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                string idString = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                id = (TID)Convert.ChangeType(idString, typeof(TID));
            }
            return id;
        }
    }
}