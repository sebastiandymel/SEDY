using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class EqualizeCommand : IFakeVerifitCommand
    {
        public JObject GetJsonResponse()
        {
            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            jobject["id"] = "equalize";
            return jobject;
        }
    }

    internal class EqualizeCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("equalize"))
            {
                return ClfsServer.ServiceLocator.Resolve<EqualizeCommand>();
            }
            return null;
        }
    }
}
