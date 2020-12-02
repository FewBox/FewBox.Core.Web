using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Claims;

namespace FewBox.Core.Web.Token
{
    public class MemoryTokenCache : TokenService
    {
        private IMemoryCache MemoryCache { get; set; }

        public MemoryTokenCache(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
        }

        public override string GenerateToken(UserProfile userProfile, DateTime expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            if(expiredTime == DateTime.MaxValue)
            {
                this.MemoryCache.Set<UserProfile>(token, userProfile);
            }
            else
            {
                this.MemoryCache.Set<UserProfile>(token, userProfile, expiredTime);
            }
            return token;
        }

        public override string GetUserIdByToken(string token)
        {
            string userId = null;
            var userProfile = this.GetUserProfileByToken(token);
            if(userProfile!=null)
            {
                userId = userProfile.Id;
            }
            return userId;
        }

        public override UserProfile GetUserProfileByToken(string token)
        {
            return this.MemoryCache.Get<UserProfile>(token);
        }

        public override bool ValidateToken(string token, string key, string issuer, string audience)
        {
            var userProfile = this.MemoryCache.Get<UserProfile>(token);
            return userProfile != null;
        }
    }
}
