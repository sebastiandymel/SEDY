using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class ServerInfoCommand : IFakeVerifitCommand
    {
        private IUiBridge bridge;

        public ServerInfoCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }

        public JObject GetJsonResponse()
        {
            var o = JObject.Parse(EmbededResource.GetFileText("serverInfo.json"));

            o["result"]["model"] = this.bridge.SelectedVerifitModel;
            o["result"]["version"] = this.bridge.VerifitDeviceFirmwareVersion;
            return o;
        }
    }

    internal class ServerInfoCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command == "serverInfo")
            {
                return ClfsServer.ServiceLocator.Resolve<ServerInfoCommand>();
            }
            return null;
        }
    }
}
