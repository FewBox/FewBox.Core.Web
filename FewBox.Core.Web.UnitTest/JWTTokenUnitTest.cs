using System;
using System.Collections.Generic;
using System.Security.Claims;
using FewBox.Core.Web.Token;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Core.Core.UnitTest
{
    [TestClass]
    public class JWTTokenUnitTest
    {
        private ITokenService TokenService { get; set; }
        private UserInfo UserInfo { get; set; }

        [TestInitialize]
        public void Init()
        {
            Guid userId = Guid.NewGuid();
            this.TokenService = new JWTToken();
            this.UserInfo = new UserInfo { 
                Id = userId.ToString(),
                Key = "EnVsakc0bNXs1UYHAiOjE1ND",
                Issuer = "https://fewbox.com",
                Claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, "landpy" ),
                    new Claim(ClaimTypes.Email, "dev@fewbox.com"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Role, "Normal")
                }
            };
        }

        [TestMethod]
        public void TestToken()
        {
            string token = this.TokenService.GenerateToken(this.UserInfo, TimeSpan.FromMinutes(5));
            Assert.AreEqual(this.UserInfo.Id, this.TokenService.GetUserIdByToken(token));
            Assert.AreEqual("landpy", this.TokenService.GetUserProfileByToken(token).Name);
            Assert.AreEqual("dev@fewbox.com", this.TokenService.GetUserProfileByToken(token).Email);
            Assert.AreEqual(2, this.TokenService.GetUserProfileByToken(token).Roles.Count);
        }
    }
}
