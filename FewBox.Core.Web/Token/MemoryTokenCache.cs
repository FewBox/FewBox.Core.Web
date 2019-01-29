using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

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
                DisplayName = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == "DisplayName").Value : String.Empty,
                Title = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == "Title").Value : String.Empty,
                Department = userInfo.Claims!=null ? userInfo.Claims.FirstOrDefault(c => c.Type == "Department").Value : String.Empty
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
