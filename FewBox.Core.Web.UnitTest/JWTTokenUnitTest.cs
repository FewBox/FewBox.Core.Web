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
using System.Linq;

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

        [TestMethod]
        public void TestTokenDecode()
        {
            string authorization = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSX0ZFV0JPWC5TRVJWSUNFLkxPV0NPREUuTElOS19GUkVFIiwiaHR0cDovL3NjaGVtYXMuZmV3Ym94LmNvbS9qd3QvaWRlbnRpdHkvY2xhaW1zL2FwaSI6Ikg0c0lBQUFBQUFBQUE1M1F3UTZDTUF3RzRIZmhiT0FaVk1TWW1FaUNub3d4Yy95T3hVSEoxamg4ZTZmY3hYRnFELzNTOWo4bkJmeUtoclNDZldxSmRFOStUWFdvdW50a3gxZFB5b3ErMFhEWkZsd0twVHUxTkNaWi9IWU4yamh4Y3JEWDB0SmRtd0JMd2JJNStBNzJIMWFCT1N5Wno4aHgxSG5ocjUzYkROcXhtM0FWazhYY0ZFY2NIV1dPbGo3TkYwMlBGMEI5RXpLTTV6QmdqUGxkM3NoWkx2UVpBZ0FBIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoieWlsIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoibWFya2V0aW5nQHlpbC5pbmsiLCJodHRwOi8vc2NoZW1hcy5mZXdib3guY29tL2p3dC9pZGVudGl0eS9jbGFpbXMvdGVuYW50IjoibWFya2V0aW5nQHlpbC5pbmsiLCJodHRwOi8vc2NoZW1hcy5mZXdib3guY29tL2p3dC9pZGVudGl0eS9jbGFpbXMvaWQiOiIzNzIwYTk2NC1iOGIxLTQwMjUtYmViMS1hMTFkMDA3ZWM5ZTMiLCJodHRwOi8vc2NoZW1hcy5mZXdib3guY29tL2p3dC9pZGVudGl0eS9jbGFpbXMvaXNzdWVyIjoiaHR0cHM6Ly9mZXdib3guY29tIiwiaHR0cDovL3NjaGVtYXMuZmV3Ym94LmNvbS9qd3QvaWRlbnRpdHkvY2xhaW1zL2F1ZGllbmNlIjoiaHR0cHM6Ly9mZXdib3guY29tIiwiZXhwIjoxNjIxNjE1NzM3LCJpc3MiOiJodHRwczovL2Zld2JveC5jb20iLCJhdWQiOiJodHRwczovL2Zld2JveC5jb20ifQ.OAKxJFjjfWvUaHYq6t4PClEXUCs2MZChuGESMYysWEw";
            string token = authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
            var userProfile = this.TokenService.GetUserProfileByToken(token);
            string service = "FewBox.Service.LowCode.Link";
            string controller = "Feedbacks";
            string action = "DeleteOwner";
            bool doesUserHavePermission = userProfile.Apis.Contains($"{service}/{controller}/{action}");
            Assert.IsTrue(doesUserHavePermission);
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