using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class GetScreenCommand : IFakeVerifitCommand
    {
        public JObject GetJsonResponse()
        {
            return JObject.Parse(EmbededResource.GetFileText("getScreen.json"));
        }
    }

    internal class GetScreenCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("getScreen"))
            {
                return ClfsServer.ServiceLocator.Resolve<GetScreenCommand>();
            }
            return null;
        }
    }
}
