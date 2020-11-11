using System;

namespace FewBox.Core.Web.Token
{
    public interface ITokenService
    {
        string GenerateToken(UserInfo userInfo);
        string GenerateToken(UserInfo userInfo, DateTime expiredTime);
        string GetUserIdByToken(string token);
        UserProfile GetUserProfileByToken(string token);
        bool ValidateToken(string token, string key, string issuer);
    }
}