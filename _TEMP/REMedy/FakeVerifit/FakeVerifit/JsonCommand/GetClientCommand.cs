using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;
using FakeVerifit;

namespace FakeVerifit
{
    public class GetClientCommand : IFakeVerifitCommand
    {
        private readonly IUiBridge bridge;

        public GetClientCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }

        public JObject GetJsonResponse()
        {
            var baseObj =  JObject.Parse(EmbededResource.GetFileText("getClient.json"));

            var leftRecd = this.bridge.RandomRecd.ConvertToJObject();
            baseObj["result"]["left"]["data"]["recd"]["data"]["recddata"]["data"] = leftRecd;

            var rightRecd = this.bridge.RandomRecd.ConvertToJObject();
            baseObj["result"]["right"]["data"]["recd"]["data"]["recddata"]["data"] = rightRecd;

            var leftUcl = this.bridge.RandomUCL.ConvertToJObject();
            baseObj["result"]["left"]["data"]["splucl"]["data"] = leftUcl;

            var rightUcl = this.bridge.RandomUCL.ConvertToJObject();
            baseObj["result"]["right"]["data"]["splucl"]["data"] = rightUcl;

            return baseObj;
        }
    }

    internal class GetClientCommandCreator : IFakeVerifitCommandCreator
    {
        public IFakeVerifitCommand Create(string command)
        {
            if (command == "getClient")
            {
                return ClfsServer.ServiceLocator.Resolve<GetClientCommand>();
            }
            return null;
        }
    }
}
