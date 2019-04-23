using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FewBox.Core.Web.Token;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        private IRemoteAuthenticationService RemoteAuthenticationService { get; set; }
        private JWTConfig JWTConfig { get; set; }

        public AuthenticationController(ITokenService tokenService, IRemoteAuthenticationService remoteAuthenticationService, JWTConfig jWTConfig)
        {
            this.TokenService = tokenService;
            this.RemoteAuthenticationService = remoteAuthenticationService;
            this.JWTConfig = jWTConfig;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public SignInResponseDto SignIn([FromBody]SignInRequestDto signInRequestDto)
        {
            IList<string> roles;
            if(this.RemoteAuthenticationService.IsValid(signInRequestDto.Username, signInRequestDto.Password, out roles))
            {
                var claims = from role in roles
                select new Claim(ClaimTypes.Role, role);
                var userInfo = new UserInfo { 
                    Id = Guid.NewGuid().ToString(),
                    Key = this.JWTConfig.Key,
                    Issuer = this.JWTConfig.Issuer,
                    Claims = claims
                };
                string token = this.TokenService.GenerateToken(userInfo, signInRequestDto.ExpiredTime);
                return new SignInResponseDto { IsValid = true, Token = token };
            }
            else
            {
                return new SignInResponseDto { IsValid = false };
            }
        }

        [AllowAnonymous]
        [HttpPost("renewtoken")]
        [RemoteRoleAuthorize(Policy="RemoteRole_Pure")]
        public RenewTokenResponseDto RenewToken([FromBody] RenewTokenRequestDto renewTokenRequestDto)
        {
            var userInfo = new UserInfo { 
                Id = Guid.NewGuid().ToString(),
                Key = this.JWTConfig.Key,
                Issuer = this.JWTConfig.Issuer,
                Claims = this.HttpContext.User.Claims
            };
            string token = this.TokenService.GenerateToken(userInfo, renewTokenRequestDto.ExpiredTime);
            return new RenewTokenResponseDto { Token = token };
        }

        [AllowAnonymous]
        [HttpGet("currentclaims")]
        public object GetCurrentClaims()
        {
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }
    }
}