using System.Linq;
using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Http;
using FewBox.Core.Utility.Converter;
using System;

namespace FewBox.Core.Web.Token
{
    public class CurrentUser<T> : ICurrentUser<T>
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private ITokenService TokenService { get; set; }
        public CurrentUser(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.TokenService = tokenService;
        }
        public T GetId()
        {
            T id = default(T);
            if (this.HttpContextAccessor.HttpContext != null)
            {
                string authorization = this.HttpContextAccessor.HttpContext.Request.Query["access_token"].Count > 0 ? this.HttpContextAccessor.HttpContext.Request.Query["access_token"] : this.HttpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (!String.IsNullOrEmpty(authorization))
                {
                    string token = authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
                    var userProfile = this.TokenService.GetUserProfileByToken(token);
                    id = TypeUtility.Converte<T>(userProfile.Id);
                }
            }
            return id;
        }
    }
}