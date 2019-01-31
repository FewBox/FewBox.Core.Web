using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Claims;

namespace FewBox.Core.Web.Token
{
    public class MemoryTokenCache : ITokenService
    {
        private IMemoryCache MemoryCache { get; set; }

        public MemoryTokenCache(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
        }

        public string GenerateToken(UserInfo userInfo, TimeSpan expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            var userProfile = new UserProfile{
                Issuer = userInfo.Issuer,
                Id = userInfo.Id!=null ? userInfo.Id.ToString() : String.Empty,
                Name = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : String.Empty,
                Email = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value : String.Empty,
                Roles = userInfo.Claims!=null ? userInfo.Claims.Where(c => c.Type== ClaimTypes.Role).Select(c => c.Value).ToList() : null
            };
            this.MemoryCache.Set<UserProfile>(token, userProfile, expiredTime);
            return token;
        }

        public string GetUserIdByToken(string token)
        {
            return this.MemoryCache.Get<UserProfile>(token).Id;
        }

        public UserProfile GetUserProfileByToken(string token)
        {
            return this.MemoryCache.Get<UserProfile>(token);
        }
    }
}
