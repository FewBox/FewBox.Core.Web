using FewBox.Core.Utility.Formatter;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;

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
                DisplayName = userInfo.Claims.FirstOrDefault(c => c.Type == "DisplayName").Value,
                Title = userInfo.Claims.FirstOrDefault(c => c.Type == "Title").Value,
                Department = userInfo.Claims.FirstOrDefault(c => c.Type == "Department").Value
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
