using FakeVerifit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FakeVerifitTests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void AppInfo_Test()
        {
            AppInfoCommand appInfoCommand = new AppInfoCommand();
            var response = appInfoCommand.GetJsonResponse();
            Assert.AreEqual("appInfo", response.Value<string>("id"));
        }
    }
}
