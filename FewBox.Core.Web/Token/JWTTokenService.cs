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
        public override string GenerateToken(UserProfile userProfile, DateTime expiredTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userProfile.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token;
            List<Claim> claims = new List<Claim>();
            if (userProfile.Roles != null && userProfile.Roles.Count > 0)
            {
                var roleClaims = userProfile.Roles.Select(role => new Claim(ClaimTypes.Role, role));
                claims.AddRange(roleClaims);
            }
            if (userProfile.Apis != null && userProfile.Apis.Count > 0)
            {
                var apiClaims = userProfile.Apis.Select(api => new Claim(TokenClaims.Api, api));
                claims.AddRange(apiClaims);
            }
            if (userProfile.Modules != null && userProfile.Modules.Count > 0)
            {
                var moduleClaims = userProfile.Modules.Select(module => new Claim(TokenClaims.Module, module));
                claims.AddRange(moduleClaims);
            }
            token = new JwtSecurityToken(
                    userProfile.Issuer,
                    userProfile.Audience,
                    claims.Union(new List<Claim>{
                        new Claim(ClaimTypes.MobilePhone, userProfile.MobilePhone==null?String.Empty:userProfile.MobilePhone),
                        new Claim(ClaimTypes.Name, userProfile.Name==null?String.Empty:userProfile.Name),
                        new Claim(ClaimTypes.Email, userProfile.Email==null?String.Empty:userProfile.Email),
                        new Claim(TokenClaims.Tenant, userProfile.Tenant==null?String.Empty:userProfile.Tenant),
                        new Claim(TokenClaims.Id, userProfile.Id==null?String.Empty:userProfile.Id),
                        new Claim(TokenClaims.Issuer, userProfile.Issuer==null?String.Empty:userProfile.Issuer),
                        new Claim(TokenClaims.Audience, userProfile.Audience==null?String.Empty:userProfile.Audience)
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
                Audience = this.GetClaimValue(jsonToken.Claims, TokenClaims.Audience),
                Name = this.GetClaimValue(jsonToken.Claims, ClaimTypes.Name),
                Email = this.GetClaimValue(jsonToken.Claims, ClaimTypes.Email),
                Roles = this.GetClaimValues(jsonToken.Claims, ClaimTypes.Role),
                Apis = this.GetClaimValues(jsonToken.Claims, TokenClaims.Api)
            };
        }

        public override bool ValidateToken(string token, string key, string issuer, string audience)
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
                ValidAudience = audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
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