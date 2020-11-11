using System;
using FewBox.Core.Web.Token;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Core.Core.UnitTest
{
    [TestClass]
    public class MemoryTokenCacheUnitTest
    {
        private ITokenService TokenService { get; set; }
        private UserInfo UserInfo { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.TokenService = new MemoryTokenCache(new MemoryCache(new MemoryCacheOptions{})); // MVC: services.AddMemoryCache();
            this.UserInfo = new UserInfo { Id = Guid.NewGuid().ToString() };
        }

        [TestMethod]
        public void TestToken()
        {
            string token = this.TokenService.GenerateToken(this.UserInfo, DateTime.Now.AddMinutes(3));
            Assert.AreEqual(this.UserInfo.Id, this.TokenService.GetUserIdByToken(token));
        }
    }
}
