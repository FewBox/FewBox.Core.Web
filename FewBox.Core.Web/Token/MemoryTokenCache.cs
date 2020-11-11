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

        public override string GenerateToken(UserInfo userInfo, DateTime expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            var userProfile = new UserProfile{
                Issuer = userInfo.Issuer,
                Id = userInfo.Id!=null ? userInfo.Id.ToString() : String.Empty,
                Name = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : String.Empty,
                Email = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value : String.Empty,
                Roles = userInfo.Claims!=null ? userInfo.Claims.Where(c => c.Type== ClaimTypes.Role).Select(c => c.Value).ToList() : null
            };
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

        public override bool ValidateToken(string token, string key, string issuer)
        {
            var userProfile = this.MemoryCache.Get<UserProfile>(token);
            return userProfile != null;
        }
    }
}
