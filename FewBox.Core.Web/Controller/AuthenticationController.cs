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
        private IAuthenticationService AuthenticationService { get; set; }
        private JWTConfig JWTConfig { get; set; }

        public AuthenticationController(ITokenService tokenService, IAuthenticationService authenticationService, JWTConfig jWTConfig)
        {
            this.TokenService = tokenService;
            this.AuthenticationService = authenticationService;
            this.JWTConfig = jWTConfig;
        }

        [HttpPost("signin")]
        public PayloadResponseDto<SignInToken> SignIn([FromBody]SignInRequestDto signInRequestDto)
        {
            IList<string> roles;
            if(this.AuthenticationService.IsValid(signInRequestDto.Username, signInRequestDto.Password, signInRequestDto.UserType, out object userId, out roles))
            {
                var claims = from role in roles
                select new Claim(ClaimTypes.Role, role);
                var userInfo = new UserInfo { 
                    Id = userId.ToString(),
                    Key = this.JWTConfig.Key,
                    Issuer = this.JWTConfig.Issuer,
                    Claims = claims
                };
                TimeSpan expiredTime;
                if(!TimeSpan.TryParse(signInRequestDto.ExpiredTimeSpan, out expiredTime))
                {
                    expiredTime = ExpireTimes.Token;
                }
                string token = this.TokenService.GenerateToken(userInfo, expiredTime);
                return new PayloadResponseDto<SignInToken>{
                    Payload = new SignInToken { IsValid = true, Token = token }
                };
            }
            else
            {
                return new PayloadResponseDto<SignInToken>{
                    Payload = new SignInToken { IsValid = false }
                };
            }
        }

        [HttpPost("renewtoken")]
        [Authorize("JWTRole_ControllerAction")]
        public PayloadResponseDto<RenewToken> RenewToken([FromBody] RenewTokenRequestDto renewTokenRequestDto)
        {
            var claims = this.HttpContext.User.Claims.Where(
                c => c.Type==ClaimTypes.Role);
            var userInfo = new UserInfo { 
                Id = Guid.NewGuid().ToString(),
                Key = this.JWTConfig.Key,
                Issuer = this.JWTConfig.Issuer,
                Claims = claims
            };
            TimeSpan expiredTime;
            if(!TimeSpan.TryParse(renewTokenRequestDto.ExpiredTimeSpan, out expiredTime))
            {
                expiredTime = ExpireTimes.Token;
            }
            string token = this.TokenService.GenerateToken(userInfo, expiredTime);
            return new PayloadResponseDto<RenewToken>{
                Payload = new RenewToken { Token = token } 
                };
        }

        [HttpGet("currentclaims")]
        [Authorize("JWTRole_ControllerAction")]
        public object GetCurrentClaims()
        {
            int i = this.User.Claims.Count();
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }
    }
}