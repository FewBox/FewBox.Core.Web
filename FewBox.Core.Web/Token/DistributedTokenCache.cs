using FewBox.Core.Utility.Formatter;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Security.Claims;

namespace FewBox.Core.Web.Token
{
    public class DistributedTokenCache : TokenService
    {
        private IDistributedCache DistributedCache { get; set; }

        public DistributedTokenCache(IDistributedCache distributedCache) : base()
        {
            this.DistributedCache = distributedCache;
        }

        public override string GenerateToken(UserInfo userInfo, DateTime expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            var userProfile = new UserProfile{
                Issuer = userInfo.Issuer,
                Id = userInfo.Id.ToString(),
                Name = userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
                Email = userInfo.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                Roles = userInfo.Claims.Where(c => c.Type== ClaimTypes.Role).Select(c => c.Value).ToList()
            };
            if(expiredTime == DateTime.MaxValue)
            {
                this.DistributedCache.SetString(token.ToString(), JsonUtility.Serialize<UserProfile>(userProfile));
            }
            else
            {
                DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(expiredTime.Ticks)
                };
                this.DistributedCache.SetString(token.ToString(), JsonUtility.Serialize<UserProfile>(userProfile), distributedCacheEntryOptions);
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
            string userProfileString = this.DistributedCache.GetString(token.ToString());
            return JsonUtility.Deserialize<UserProfile>(userProfileString);
        }

        public override bool ValidateToken(string token, string key, string issuer, string audience)
        {
            string userProfileString = this.DistributedCache.GetString(token.ToString());
            return !String.IsNullOrEmpty(userProfileString);
        }
    }
}
