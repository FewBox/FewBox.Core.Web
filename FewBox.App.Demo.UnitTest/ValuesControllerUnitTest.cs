using Microsoft.VisualStudio.TestTools.UnitTesting;
using FewBox.Core.Web.Token;
using Moq;
using System;
using FewBox.Core.Web.Config;

namespace FewBox.App.Demo.UnitTest
{
    [TestClass]
    public class ValuesControllerUnitTest
    {
        private ITokenService AdminTokenService { get; set; }
        private ITokenService NormalTokenService { get; set; }
        private JWTConfig JWTConfig { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.AdminTokenService = Moq.Mock.Of<ITokenService>(l=>l.GenerateToken(
                It.IsAny<UserInfo>(),
                It.IsAny<TimeSpan>())=="");
            this.NormalTokenService = Moq.Mock.Of<ITokenService>(l=>l.GenerateToken(
                It.IsAny<UserInfo>(),
                It.IsAny<TimeSpan>())=="");
            this.JWTConfig = new JWTConfig{
                Key = "EnVsakc0bNXs1UYHAiOjE1ND",
                Issuer = "https://fewbox.com"
            };
        }

        [TestMethod]
        public void TestMethod1()
        {
            //var tokenController = new AuthenticationController(this.AdminTokenService, this.JWTConfig);
        }
    }
}
