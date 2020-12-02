using System;
using System.Collections.Generic;
using System.Reflection;
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
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class PayloadController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        private FewBoxConfig FewBoxConfig { get; set; }

        public PayloadController(ITokenService tokenService, FewBoxConfig fewBoxConfig)
        {
            this.TokenService = tokenService;
            this.FewBoxConfig = fewBoxConfig;
        }

        [HttpGet("GetAdminToken")]
        [AllowAnonymous]
        public PayloadResponseDto<string> GetAdminToken()
        {
            string service = Assembly.GetEntryAssembly().GetName().Name;
            var userProfile = new UserProfile
            {
                Tenant = "FewBox",
                Id = "UserId",
                Key = this.FewBoxConfig.JWT.Key,
                Issuer = this.FewBoxConfig.JWT.Issuer,
                Audience = this.FewBoxConfig.JWT.Audience,
                Roles = new List<string> { "Admin" },
                Apis = new List<string> { $"{service}/Payload/Validate" }
            };
            var expires = DateTime.Now.AddMinutes(1);
            string token = this.TokenService.GenerateToken(userProfile, expires);
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

        [HttpGet("Validate403")]
        public PayloadResponseDto<string> Validate403()
        {
            return new PayloadResponseDto<string>
            {
                Payload = $"Hello 403!"
            };
        }
    }
}