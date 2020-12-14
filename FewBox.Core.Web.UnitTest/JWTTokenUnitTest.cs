using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using FewBox.Core.Web.Token;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FewBox.Core.Core.UnitTest
{
    [TestClass]
    public class JWTTokenUnitTest
    {
        private string Key { get; set; }
        private string Issuer { get; set; }
        private string Audience { get; set; }
        private ITokenService TokenService { get; set; }
        private UserProfile UserProfile { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.Key = "EnVsakc0bNXs1UYHAiOjE1ND";
            this.Issuer = "https://fewbox.com";
            this.Audience = "https://figma.fewbox.com";
            Guid userId = Guid.NewGuid();
            var loggerMock = new Mock<ILogger<JWTTokenService>>();
            var list = new List<string>();
            loggerMock.Setup(l => l.Log<JWTTokenService>(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<JWTTokenService>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<JWTTokenService, Exception, string>>()))
                .Callback(
                    delegate (LogLevel logLevel, EventId eventId, JWTTokenService state, Exception exception, Func<JWTTokenService, Exception, string> formatter)
                    {
                        list.Add(state.ToString());
                    }
                );
            this.TokenService = new JWTTokenService(loggerMock.Object);
            this.UserProfile = new UserProfile
            {
                Tenant = "FewBox",
                Id = userId.ToString(),
                Key = this.Key,
                Issuer = this.Issuer,
                Audience = this.Audience,
                Name = "landpy",
                Email = "test@fewbox.com",
                Roles = new List<string> { "Admin", "Nomal" },
                Apis = new List<string> { "Service/Controller/Action" },
                Modules = new List<string> { "Service/Module" }
            };
        }

        [TestMethod]
        public void TestToken()
        {
            string token = this.TokenService.GenerateToken(this.UserProfile, DateTime.Now.AddDays(5));
            Console.WriteLine(token);
            Assert.AreEqual(this.UserProfile.Id.ToString(), this.TokenService.GetUserIdByToken(token));
            Assert.AreEqual("landpy", this.TokenService.GetUserProfileByToken(token).Name);
            Assert.AreEqual("test@fewbox.com", this.TokenService.GetUserProfileByToken(token).Email);
            var userProfile = this.TokenService.GetUserProfileByToken(token);
            Assert.AreEqual(2, userProfile.Roles.Count);
            Assert.AreEqual(1, userProfile.Apis.Count);
            Assert.AreEqual(1, userProfile.Modules.Count);
        }

        //[TestMethod]
        public void TestExpiredToken()
        {
            string token = this.TokenService.GenerateToken(this.UserProfile, DateTime.Now.AddSeconds(1));
            Assert.IsTrue(this.TokenService.ValidateToken(token, this.Key, this.Issuer, this.Audience));
            Thread.Sleep(1000);
            Assert.IsFalse(this.TokenService.ValidateToken(token, this.Key, this.Issuer, this.Audience));
        }

        [TestMethod]
        public void TestTokenLengthGe16()
        {
            var userProfile = new UserProfile
            {
                Tenant = "FewBox",
                Id = Guid.NewGuid().ToString(),
                Key = "1234567890123456",
                Issuer = this.Issuer,
                Audience = this.Audience
            };
            Assert.IsNotNull(this.TokenService.GenerateToken(userProfile));
        }

        [TestMethod]
        public void TestTokenLengthLe16()
        {
            UserProfile userProfile;
            Assert.ThrowsException<UserInfoKeyLengthException>(() => userProfile = new UserProfile
            {
                Id = Guid.NewGuid().ToString(),
                Key = "123456789012345",
                Issuer = this.Issuer
            });
        }

        //[TestMethod]
        public void TestTokenDecode()
        {
            string authorization = "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSX1NoaXBwaW5nX1NVUFBFUkFETUlOIiwiaHR0cDovL3NjaGVtYXMuZmV3Ym94LmNvbS9qd3QvMjAxOS8wNC9pZGVudGl0eS9jbGFpbXMvaWQiOiIzZmM3YzhjOC03N2UyLTQyOTYtODMwYS1iM2FhZDdlYTkxYjAiLCJodHRwOi8vc2NoZW1hcy5mZXdib3guY29tL2p3dC8yMDE5LzA0L2lkZW50aXR5L2NsYWltcy9pc3N1ZXIiOiJodHRwczovL2Zld2JveC5jb20iLCJleHAiOjE1NzIxNTUxMzgsImlzcyI6Imh0dHBzOi8vZmV3Ym94LmNvbSIsImF1ZCI6Imh0dHBzOi8vZmV3Ym94LmNvbSJ9.n8E4WTP5ZldVFJM-wPvv1zvUJcteTAg3nOifhErWk_k";
            string token = authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
            var userInfo = this.TokenService.GetUserProfileByToken(token);
        }

        //[TestMethod]
        public void Test()
        {
            TokenValidationParameters validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://fewbox.com",
                ValidAudience = "https://fewbox.com",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Design & Deliver App Service by Love"))
            };
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var user = handler.ValidateToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLmZld2JveC5jb20vand0LzIwMTkvMDQvaWRlbnRpdHkvY2xhaW1zL2lkIjoiMDAwMDAwMDAtMDAwMC0wMDAwLTAwMDAtMDAwMDAwMDAwMDAwIiwiaHR0cDovL3NjaGVtYXMuZmV3Ym94LmNvbS9qd3QvMjAxOS8wNC9pZGVudGl0eS9jbGFpbXMvaXNzdWVyIjoiaHR0cHM6Ly9mZXdib3guY29tIiwiZXhwIjoxNTg3OTEzMTc4LCJpc3MiOiJodHRwczovL2Zld2JveC5jb20iLCJhdWQiOiJodHRwczovL2Zld2JveC5jb20ifQ.5yVozbLjORmlvhs6PTjbGd_qAT6gDOOTIMC5VVKkGis", validationParameters, out validatedToken);
        }
    }
}