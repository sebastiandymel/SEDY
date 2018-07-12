using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class AppInfoCommand : IFakeVerifitCommand
    {
        public JObject GetJsonResponse()
        {
            return JObject.Parse(EmbededResource.GetFileText("appInfo.json"));
        }
    }

    internal class AppInfoCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command == "appInfo speechmap")
            {
                return ClfsServer.ServiceLocator.Resolve<AppInfoCommand>();
            }
            return null;
        }
    }
}
