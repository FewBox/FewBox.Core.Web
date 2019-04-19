using Microsoft.VisualStudio.TestTools.UnitTesting;
using FewBox.App.Demo.Controllers;
using FewBox.Core.Web.Token;
using FewBox.App.Demo.Configs;
using System.Linq;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace FewBox.App.Demo.UnitTest
{
    [TestClass]
    public class ValuesControllerUnitTest
    {
        private ITokenService AdminTokenService { get; set; }
        private ITokenService NormalTokenService { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.AdminTokenService = Moq.Mock.Of<ITokenService>(l=>l.GenerateToken(
                It.IsAny<UserInfo>(),
                It.IsAny<TimeSpan>())=="");
            this.NormalTokenService = Moq.Mock.Of<ITokenService>(l=>l.GenerateToken(
                It.IsAny<UserInfo>(),
                It.IsAny<TimeSpan>())=="");
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
