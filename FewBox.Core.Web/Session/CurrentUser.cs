using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
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
            var claim = this.HttpContextAccessor.HttpContext.User.Claims.Where(p=>p.Type==TokenClaims.Id).FirstOrDefault();
            if(claim!=null)
            {
                id = TypeUtility.Converte<T>(claim.Value);
            }
            return id;
        }
    }
}