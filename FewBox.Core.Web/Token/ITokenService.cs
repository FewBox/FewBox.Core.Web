using System;

namespace FewBox.Core.Web.Token
{
    public interface ITokenService
    {
        string GenerateToken(UserProfile userProfile);
        string GenerateToken(UserProfile userProfile, DateTime expiredTime);
        string GetUserIdByToken(string token);
        UserProfile GetUserProfileByToken(string token);
        bool ValidateToken(string token, string key, string issuer, string audience);
    }
}