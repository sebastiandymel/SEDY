using Autofac;
using FakeVerifit;
using FakeVerifit.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FakeVerifitTests
{
    [TestClass()]
    public class FakeVerifitCommandTests
    {
        private FakeVerifitCommandFactory factory;

        [TestInitialize()]
        public void Initialize()
        {
            var container = Bootstraper.CreateContainer(new UiModule());

            this.factory = container.Resolve<FakeVerifitCommandFactory>();

            Assert.IsNotNull(this.factory);
        }
    
    }
}