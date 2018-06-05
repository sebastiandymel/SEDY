using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class CheckVersionCommand : IFakeVerifitCommand
    {
        IUiBridge bridge;
        public CheckVersionCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }
        public JObject GetJsonResponse()
        {
            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            jobject["id"] = "checkVersion";

            if (this.bridge.IncompatibleVersion)
            { 
                jobject["result"] = "0";
            }

            return jobject;
        }
    }

    internal class CheckVersionCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("checkVersion"))
            {
                return ClfsServer.ServiceLocator.Resolve<CheckVersionCommand>();
            }
            return null;
        }
    }
}
