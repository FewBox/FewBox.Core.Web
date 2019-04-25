using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using FewBox.Core.Web.Token;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Core.Core.UnitTest
{
    [TestClass]
    public class JWTTokenUnitTest
    {
        private string Key { get; set; }
        private string Issuer { get; set; }
        private ITokenService TokenService { get; set; }
        private UserInfo UserInfo { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.Key = "EnVsakc0bNXs1UYHAiOjE1ND";
            this.Issuer = "https://fewbox.com";
            Guid userId = Guid.NewGuid();
            this.TokenService = new JWTToken();
            this.UserInfo = new UserInfo { 
                Id = userId.ToString(),
                Key = this.Key,
                Issuer = this.Issuer,
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

        [TestMethod]
        public void TestExpiredToken()
        {
            TimeSpan expiredTime = TimeSpan.FromSeconds(1);
            string token = this.TokenService.GenerateToken(this.UserInfo, expiredTime);
            Assert.IsTrue(this.TokenService.ValidateToken(token, this.Key, this.Issuer));
            Thread.Sleep(expiredTime);
            Assert.IsFalse(this.TokenService.ValidateToken(token, this.Key, this.Issuer));
        }

        [TestMethod]
        public void TestTokenLengthGe16()
        {
            var userInfo = new UserInfo { 
                Id = Guid.NewGuid().ToString(),
                Key = "1234567890123456",
                Issuer = this.Issuer,
                Claims = new List<Claim>{
                }
            };
            Assert.IsNotNull(this.TokenService.GenerateToken(userInfo));
        }

        [TestMethod]
        public void TestTokenLengthLe16()
        {
            UserInfo userInfo;
            Assert.ThrowsException<UserInfoKeyLengthException>(()=> userInfo = new UserInfo { 
                Id = Guid.NewGuid().ToString(),
                Key = "123456789012345",
                Issuer = this.Issuer,
                Claims = new List<Claim>{
                }
            });
        }
    }
}