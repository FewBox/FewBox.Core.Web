using System;
using System.Collections.Generic;
using System.Security.Claims;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Demo.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private ITokenService TokenService { get; set; }

        public AuthController(ITokenService tokenService)
        {
            this.TokenService = tokenService;
        }
        [HttpGet]
        public PayloadResponseDto<string> Get()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
            var userInfo = new UserInfo
            {
                Tenant = "FewBox",
                Id = "UserId",
                Key = "DeliveryTheAppByLove",
                Issuer = "Issuer",
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, DateTime.Now.AddHours(1));
            return new PayloadResponseDto<string>
            {
                Payload = $"Bearer {token}"
            };
        }
    }
}