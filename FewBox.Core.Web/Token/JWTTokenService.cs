using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Token
{
    public class JWTTokenService : TokenService
    {
        private ILogger<TokenService> Logger { get; set; }
        private JwtSecurityTokenHandler JwtSecurityTokenHandler { get; set; }
        public JWTTokenService(ILogger<JWTTokenService> logger)
        {
            this.Logger = logger;
            this.JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        public override string GenerateToken(UserInfo userInfo, DateTime expiredTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userInfo.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token;
            token = new JwtSecurityToken(
                    userInfo.Issuer,
                    userInfo.Issuer,
                    userInfo.Claims.Union(new List<Claim>{
                        new Claim(TokenClaims.Tenant, userInfo.Tenant),
                        new Claim(TokenClaims.Id, userInfo.Id.ToString()),
                        new Claim(TokenClaims.Issuer, userInfo.Issuer)
                    }),
                    expires: expiredTime,
                    signingCredentials: creds);
            return this.JwtSecurityTokenHandler.WriteToken(token);
        }

        public override string GetUserIdByToken(string token)
        {
            var jsonToken = this.JwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            return jsonToken.Claims.FirstOrDefault(c => c.Type == TokenClaims.Id).Value;
        }

        public override UserProfile GetUserProfileByToken(string token)
        {
            var jsonToken = this.JwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            return new UserProfile
            {
                Tenant = this.GetClaimValue(jsonToken.Claims, TokenClaims.Tenant),
                Id = this.GetClaimValue(jsonToken.Claims, TokenClaims.Id),
                Issuer = this.GetClaimValue(jsonToken.Claims, TokenClaims.Issuer),
                Name = this.GetClaimValue(jsonToken.Claims, ClaimTypes.Name),
                Email = this.GetClaimValue(jsonToken.Claims, ClaimTypes.Email),
                Roles = this.GetClaimValues(jsonToken.Claims, ClaimTypes.Role),
                Apis = this.GetClaimValues(jsonToken.Claims, TokenClaims.Api)
            };
        }

        public override bool ValidateToken(string token, string key, string issuer)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                // Validation states:
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                // Validation parameters:
                ValidIssuer = issuer,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = securityKey
            };
            ClaimsPrincipal claimsPricipal = null;
            try
            {
                SecurityToken securityToken;
                claimsPricipal = this.JwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception exception)
            {
                this.Logger.LogError(exception.Message);
            }
            return claimsPricipal != null;
        }
        private string GetClaimValue(IEnumerable<Claim> claims, string name)
        {
            string value = String.Empty;
            var claim = claims.FirstOrDefault(c => c.Type == name);
            if (claim != null)
            {
                value = claim.Value;
            }
            return value;
        }

        private IList<string> GetClaimValues(IEnumerable<Claim> claims, string name)
        {
            return claims.Where(c => c.Type == name).Select(c => c.Value).ToList();
        }
    }
}