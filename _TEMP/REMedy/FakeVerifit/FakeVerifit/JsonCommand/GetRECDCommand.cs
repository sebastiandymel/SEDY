using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;
using FakeVerifit;

namespace FakeVerifit
{
    public class GetRECDCommand : IFakeVerifitCommand
    {
        private readonly IUiBridge bridge;

        public GetRECDCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }

        public JObject GetJsonResponse()
        {
            var baseObj =  JObject.Parse(EmbededResource.GetFileText("getRECD.json"));
            baseObj["result"]["recddata"]["data"] = this.bridge.RandomRecd.ConvertToJObject();
            return baseObj;
        }
    }

    internal class GetRECDCommandCreator : IFakeVerifitCommandCreator
    {
        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("getRECD"))
            {
                return ClfsServer.ServiceLocator.Resolve<GetRECDCommand>();
            }
            return null;
        }
    }
}
