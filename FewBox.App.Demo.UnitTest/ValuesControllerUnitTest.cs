using System;
using System.Linq;
using System.Collections.Generic;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;

namespace FewBox.App.Demo.UnitTest
{
    [TestClass]
    public class ValuesControllerUnitTest
    {
        private IRemoteAuthenticationService RemoteAuthenticationService { get; set; }
        private JWTConfig JWTConfig { get; set; }

        [TestInitialize]
        public void Init()
        {
            IList<string> roles = new List<string> { "Admin" };
            this.RemoteAuthenticationService = Mock.Of<IRemoteAuthenticationService>(l=>
                l.FindRolesByControllerAndAction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IHeaderDictionary>())==new List<string>{} &&
                l.IsValid(It.IsAny<string>(), It.IsAny<string>(), out roles ) == true);
            this.JWTConfig = new JWTConfig{
                Key = "EnVsakc0bNXs1UYHAiOjE1ND",
                Issuer = "https://fewbox.com"
            };
        }

        [TestMethod]
        public void TestSignInAndValues()
        {
            var tokenService = new JWTToken();
            var authenticationController = new AuthenticationController(tokenService, this.RemoteAuthenticationService, this.JWTConfig);
            authenticationController.ControllerContext.HttpContext = new DefaultHttpContext();
            var signInResponseDto = authenticationController.SignIn(new SignInRequestDto{ Username = "landpy", Password = "fewbox", ExpiredTime = TimeSpan.FromMinutes(5) });
            Assert.IsTrue(signInResponseDto.IsSuccessful);
            Assert.IsTrue(signInResponseDto.IsValid);
            Assert.IsNotNull(signInResponseDto.Token);
            var userProfile = tokenService.GetUserProfileByToken(signInResponseDto.Token);
            Assert.IsTrue(userProfile.Roles.Contains("Admin"));
            authenticationController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {signInResponseDto.Token}";
            authenticationController.ControllerContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]{new Claim(ClaimTypes.Role, "Admin"), new Claim(TokenClaims.Id, userProfile.Id), new Claim(TokenClaims.Issuer, userProfile.Issuer)},
                "JWT"));
            var renewTokenResponseDto = authenticationController.RenewToken(new RenewTokenRequestDto { ExpiredTime = TimeSpan.FromMinutes(5) });
            Assert.IsTrue(renewTokenResponseDto.IsSuccessful);
            Assert.IsNotNull(renewTokenResponseDto.Token);
        }
    }
}
