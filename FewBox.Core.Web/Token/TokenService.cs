using System;

namespace FewBox.Core.Web.Token
{
    public abstract class TokenService : ITokenService
    {
        public string GenerateToken(UserProfile userProfile)
        {
            return GenerateToken(userProfile, DateTime.MaxValue);
        }
        public abstract string GenerateToken(UserProfile userProfile, DateTime expiredTime);
        public abstract string GetUserIdByToken(string token);
        public abstract UserProfile GetUserProfileByToken(string token);
        public abstract bool ValidateToken(string token, string key, string issuer, string audience);
    }
}