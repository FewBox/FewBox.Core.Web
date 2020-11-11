using System;
using System.Collections.Generic;
using System.Security.Claims;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Demo.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy="JWTPayload_ControllerAction")]
    public class PayloadController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        private FewBoxConfig FewBoxConfig {get;set;}

        public PayloadController(ITokenService tokenService, FewBoxConfig fewBoxConfig)
        {
            this.TokenService = tokenService;
            this.FewBoxConfig = fewBoxConfig;
        }

        [HttpGet("GetAdminToken")]
        [AllowAnonymous]
        public PayloadResponseDto<string> GetAdminToken()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin"), new Claim(TokenClaims.Api, "Payload/Validate") };
            var userInfo = new UserInfo
            {
                Tenant = "FewBox",
                Id = "UserId",
                Key = this.FewBoxConfig.JWT.Key,
                Issuer = this.FewBoxConfig.JWT.Issuer,
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, DateTime.Now.AddHours(1));
            return new PayloadResponseDto<string>
            {
                Payload = $"Bearer {token}"
            };
        }

        [HttpGet("Validate")]
        public PayloadResponseDto<string> Validate()
        {
            return new PayloadResponseDto<string>
            {
                Payload = $"Hello World!"
            };
        }
    }
}