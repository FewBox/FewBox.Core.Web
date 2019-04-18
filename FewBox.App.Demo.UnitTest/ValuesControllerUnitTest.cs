using Microsoft.VisualStudio.TestTools.UnitTesting;
using FewBox.App.Demo.Controllers;

namespace FewBox.App.Demo.UnitTest
{
    [TestClass]
    public class ValuesControllerUnitTest
    {
        private TokenController TokenController { get; set; }
        private ValuesController ValuesController { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.TokenController = new TokenController();
            this.ValuesController = new TokenController();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
