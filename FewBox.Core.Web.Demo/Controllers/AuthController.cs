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
            var userProfile = new UserProfile
            {
                Tenant = "FewBox",
                Id = "UserId",
                Key = "DeliveryTheAppByLove",
                Issuer = "Issuer",
                Roles = new List<string> { "Admin" }
            };
            string token = this.TokenService.GenerateToken(userProfile, DateTime.Now.AddHours(1));
            return new PayloadResponseDto<string>
            {
                Payload = $"Bearer {token}"
            };
        }
    }
}