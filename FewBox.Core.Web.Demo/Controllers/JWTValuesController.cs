using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FewBox.Core.Web.Token;
using FewBox.Core.Web.Config;
using System;

namespace FewBox.Core.Web.Controllers
{
    // Header: Authorization, Bearer [JWT]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class JWTValuesController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        private FewBoxConfig FewBoxConfig { get; set; }
        private IList<Value> Values { get; set; }

        public JWTValuesController(ITokenService tokenService, FewBoxConfig fewboxConfig)
        {
            this.TokenService = tokenService;
            this.FewBoxConfig = fewboxConfig;
            this.Values = new List<Value> {
                new Value { Id = 1, Content = "Value1" },
                new Value { Id = 2, Content = "Value2" }
            };
        }

        /// <summary>
        /// Get admin token.
        /// </summary>
        /// <returns>Token.</returns>
        [HttpGet("getadmintoken")]
        public string GetAdminToken()
        {
            Guid userId = Guid.Empty;
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
            var userInfo = new UserInfo
            {
                Tenant = "FewBox",
                Id = userId.ToString(),
                Key = this.FewBoxConfig.JWT.Key,
                Issuer = this.FewBoxConfig.JWT.Issuer,
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, DateTime.Now.AddHours(1));
            return $"Bearer {token}";
        }

        /// <summary>
        /// Get normal token.
        /// </summary>
        /// <returns>Token.</returns>
        [HttpGet("getnormaltoken")]
        public string GetNormalToken()
        {
            Guid userId = Guid.Empty;
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Normal") };
            var userInfo = new UserInfo
            {
                Tenant = "FewBox",
                Id = userId.ToString(),
                Key = this.FewBoxConfig.JWT.Key,
                Issuer = this.FewBoxConfig.JWT.Issuer,
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, DateTime.Now.AddHours(1));
            return $"Bearer {token}";
        }

        /// <summary>
        /// Verify controller and action [JWTRole_ControllerAction Policy].
        /// </summary>
        /// <returns>Values.</returns>
        [HttpGet("controllerandaction")]
        [Authorize(Policy = "JWTRole_ControllerAction")]
        public IList<Value> VerifyControllerAndAction()
        {
            return this.Values;
        }

        /// <summary>
        /// Verify method(Verb) [JWTRole_Verb Policy].
        /// </summary>
        /// <returns>Values.</returns>
        [HttpGet("method")]
        [Authorize(Policy = "JWTRole_Verb")]
        public IList<Value> VerifyMethod()
        {
            return this.Values;
        }

        /// <summary>
        /// Verify normal and admin roles [Default Claim Policy].
        /// </summary>
        /// <returns>Values.</returns>
        [HttpGet]
        [Authorize(Roles = "Normal, Admin")]
        public IList<Value> Get()
        {
            return this.Values;
        }

        /// <summary>
        /// Verify normal and admin roles [Default Claim Policy].
        /// </summary>
        /// <returns>Value.</returns>
        [Authorize(Roles = "Normal, Admin")]
        [HttpGet("{id}")]
        public Value Get(int id)
        {
            return this.Values.Where(p => p.Id == id).SingleOrDefault();
        }

        /// <summary>
        /// Verify admin roles [Default Claim Policy].
        /// </summary>
        /// <returns>Values.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public dynamic Post([FromBody] Value value)
        {
            return new { IsSuccessful = true };
        }

        /// <summary>
        /// Verify admin roles [Default Claim Policy].
        /// </summary>
        /// <returns>Values.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public dynamic Put(int id, [FromBody] Value value)
        {
            return new { IsSuccessful = true };
        }

        /// <summary>
        /// Verify admin roles [Default Claim Policy].
        /// </summary>
        /// <returns>Values.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public dynamic Delete(int id)
        {
            return new { IsSuccessful = true };
        }

        public class Value
        {
            public int Id { get; set; }
            public string Content { get; set; }
        }
    }
}