using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;
using FakeVerifit;

namespace FakeVerifit
{
    public class StatusCommand : IFakeVerifitCommand
    {
        private readonly IUiBridge bridge;
        private readonly IMeasurementService measurementService;

        public StatusCommand(IUiBridge bridge, IMeasurementService measurementService)
        {
            this.bridge = bridge;
            this.measurementService = measurementService;
        }

        public JObject GetJsonResponse()
        {
            if (this.measurementService.IsRunning)
            {
                var o = JObject.Parse(EmbededResource.GetFileText("status_busy.json"));
                o["result"]["data"]["ltass"]["data"] = this.bridge.RandomLTASS.ConvertToJObject();
                o["result"]["data"]["percentile99"]["data"] = this.bridge.RandomPercentile99.ConvertToJObject();
                o["result"]["data"]["percentile30"]["data"] = this.bridge.RandomPercentile30.ConvertToJObject();
                return o;
            }
            else
            {
                return JObject.Parse(EmbededResource.GetFileText("status_idle.json"));
            }
        }
    }

    internal class StatusCommandCreator : IFakeVerifitCommandCreator
    {
        public IFakeVerifitCommand Create(string command)
        {
            if (command == "status")
            {
                return ClfsServer.ServiceLocator.Resolve<StatusCommand>();
            }
            return null;
        }
    }
}
