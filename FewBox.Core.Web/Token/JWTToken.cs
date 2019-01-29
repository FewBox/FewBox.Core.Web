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
                    new Claim("Id", userInfo.Id.ToString(), "Id"),
                    new Claim("Issuer", userInfo.Issuer, "Issuer" )
                }),
                expires: DateTime.Now.AddTicks(expiredTime.Ticks),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetUserIdByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken.Claims.FirstOrDefault(c => c.Type == "Id").Value;
        }

        public UserProfile GetUserProfileByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return new UserProfile{
                Id = this.GetClaimValue(jsonToken.Claims, "Id"),
                Issuer = this.GetClaimValue(jsonToken.Claims, "Issuer"),
                DisplayName = this.GetClaimValue(jsonToken.Claims, "DisplayName"),
                Title = this.GetClaimValue(jsonToken.Claims, "Title"),
                Department = this.GetClaimValue(jsonToken.Claims, "Department")
            };
        }

        private string GetClaimValue(IEnumerable<Claim> claims, string name)
        {
            string value = String.Empty;
            var claim = claims.FirstOrDefault(c => c.Type == name);
            if(claim!=null)
            {
                value = claim.Value;
            }
            return value;
        }
    }
}