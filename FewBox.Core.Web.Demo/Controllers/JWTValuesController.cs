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
    [Route("api/[controller]")]
    public class JWTValuesController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        private JWTConfig JWTConfig { get; set; }
        private IList<Value> Values { get; set; }

        public JWTValuesController(ITokenService tokenService, JWTConfig jWTConfig)
        {
            this.TokenService = tokenService;
            this.JWTConfig = jWTConfig;
            this.Values = new List<Value> {
                new Value { Id = 1, Content = "Value1" },
                new Value { Id = 2, Content = "Value2" }
            };
        }

        [HttpGet("gettoken")]
        public string GetToken()
        {
            Guid userId = Guid.Empty;
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
            var userInfo = new UserInfo
            {
                Id = userId.ToString(),
                Key = this.JWTConfig.Key,
                Issuer = this.JWTConfig.Issuer,
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, TimeSpan.FromHours(1));
            return $"Bearer {token}";
        }

        [HttpGet("controllerandaction")]
        [Authorize(Policy = "JWTRole_ControllerAction")]
        public IList<Value> GetByWithHeader()
        {
            return this.Values;
        }

        [HttpGet("method")]
        [Authorize(Policy = "JWTRole_Method")]
        public IList<Value> GetByWithCookie()
        {
            return this.Values;
        }

        // GET api/values
        [HttpGet]
        [Authorize(Roles = "Normal, Admin")]
        public IList<Value> Get()
        {
            return this.Values;
        }

        // GET api/values/5
        [Authorize(Roles = "Normal, Admin")]
        [HttpGet("{id}")]
        public Value Get(int id)
        {
            return this.Values.Where(p => p.Id == id).SingleOrDefault();
        }

        // POST api/values
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public dynamic Post([FromBody] Value value)
        {
            return new { IsSuccessful = true };
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public dynamic Put(int id, [FromBody] Value value)
        {
            return new { IsSuccessful = true };
        }

        // DELETE api/values/5
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