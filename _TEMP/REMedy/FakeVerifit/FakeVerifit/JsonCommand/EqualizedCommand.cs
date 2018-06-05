using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class EqualizedCommand : IFakeVerifitCommand
    {
        public JObject GetJsonResponse()
        {
            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            jobject["id"] = "equalized";
            return jobject;
        }
    }

    internal class EqualizedCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("equalized"))
            {
                return ClfsServer.ServiceLocator.Resolve<EqualizedCommand>();
            }
            return null;
        }
    }
}
