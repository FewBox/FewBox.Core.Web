using FewBox.Core.Utility.Formatter;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Security.Claims;

namespace FewBox.Core.Web.Token
{
    public class DistributedTokenCache : ITokenService
    {
        private IDistributedCache DistributedCache { get; set; }

        public DistributedTokenCache(IDistributedCache distributedCache)
        {
            this.DistributedCache = distributedCache;
        }

        public string GenerateToken(UserInfo userInfo, TimeSpan expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiredTime
            };
            var userProfile = new UserProfile{
                Issuer = userInfo.Issuer,
                Id = userInfo.Id.ToString(),
                Name = userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                Email = userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                Roles = userInfo.Claims.Where(c => c.Type== ClaimTypes.Role).Select(c => c.Value).ToList()
            };
            this.DistributedCache.SetString(token.ToString(), JsonUtility.Serialize<UserProfile>(userProfile), distributedCacheEntryOptions);
            return token;
        }

        public string GetUserIdByToken(string token)
        {
            string userProfileString = this.DistributedCache.GetString(token.ToString());
            return JsonUtility.Deserialize<UserProfile>(userProfileString).Id;
        }

        public UserProfile GetUserProfileByToken(string token)
        {
            string userProfileString = this.DistributedCache.GetString(token.ToString());
            return JsonUtility.Deserialize<UserProfile>(userProfileString);
        }
    }
}
