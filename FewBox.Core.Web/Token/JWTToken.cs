using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace FewBox.Core.Web.Token
{
    public class JWTToken : ITokenService
    {
        public string GenerateToken(UserInfo userInfo, TimeSpan expiredTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userInfo.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                userInfo.Issuer,
                userInfo.Issuer,
                userInfo.Claims.Union( new List<Claim>{ 
                    new Claim(TokenClaims.Id, userInfo.Id.ToString()),
                    new Claim(TokenClaims.Issuer, userInfo.Issuer)
                }),
                expires: DateTime.Now.AddTicks(expiredTime.Ticks),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetUserIdByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken.Claims.FirstOrDefault(c => c.Type == TokenClaims.Id).Value;
        }

        public UserProfile GetUserProfileByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return new UserProfile{
                Id = this.GetClaimValue(jsonToken.Claims, TokenClaims.Id),
                Issuer = this.GetClaimValue(jsonToken.Claims, TokenClaims.Issuer),
                Name = this.GetClaimValue(jsonToken.Claims, ClaimTypes.Name),
                Email = this.GetClaimValue(jsonToken.Claims, ClaimTypes.Email),
                Roles = this.GetClaimValues(jsonToken.Claims, ClaimTypes.Role)
            };
        }

        private string GetClaimValue(IEnumerable<Claim> claims, string name)
        {
            string value = String.Empty;
            var claim = claims.FirstOrDefault(c => c.Type == name);
            if(claim != null)
            {
                value = claim.Value;
            }
            return value;
        }

        private IList<string> GetClaimValues(IEnumerable<Claim> claims, string name)
        {
            return claims.Where(c => c.Type== name).Select(c => c.Value).ToList();
        }
    }
}