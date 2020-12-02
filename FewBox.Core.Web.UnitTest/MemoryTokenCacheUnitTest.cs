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
        private UserProfile UserProfile { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.TokenService = new MemoryTokenCache(new MemoryCache(new MemoryCacheOptions{})); // MVC: services.AddMemoryCache();
            this.UserProfile = new UserProfile { Id = Guid.NewGuid().ToString() };
        }

        [TestMethod]
        public void TestToken()
        {
            string token = this.TokenService.GenerateToken(this.UserProfile, DateTime.Now.AddMinutes(3));
            Assert.AreEqual(this.UserProfile.Id, this.TokenService.GetUserIdByToken(token));
        }
    }
}
