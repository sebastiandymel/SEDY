using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class CancelCommand : IFakeVerifitCommand
    {
        private readonly IMeasurementService measurementService;
        public CancelCommand(IMeasurementService measurementService)
        {
            this.measurementService = measurementService;
        }

        public JObject GetJsonResponse()
        {
            this.measurementService.CancelMeasurement();
            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            jobject["id"] = "cancel";
            return jobject;
        }
    }

    internal class CancelCommandCreator : IFakeVerifitCommandCreator
    {
        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("cancel"))
            {
                return ClfsServer.ServiceLocator.Resolve<CancelCommand>();
            }
            return null;
        }
    }
}