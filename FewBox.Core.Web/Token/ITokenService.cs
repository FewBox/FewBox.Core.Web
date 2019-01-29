using System;

namespace FewBox.Core.Web.Token
{
    public interface ITokenService
    {
        string GenerateToken(UserInfo userInfo, TimeSpan expiredTime);
        string GetUserIdByToken(string token);
        UserProfile GetUserProfileByToken(string token);
    }
}