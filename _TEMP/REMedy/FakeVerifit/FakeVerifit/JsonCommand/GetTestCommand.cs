using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    internal class GetTestCommand : CurveCommandBase, IFakeVerifitCommand
    {
        private IUiBridge bridge;

        public GetTestCommand(IUiBridge bridge) : base(bridge)
        {
            this.bridge = bridge;
        }

        public JObject GetJsonResponse()
        {
            var o = JObject.Parse(EmbededResource.GetFileText("getTest.json"));

            o["result"]["targets"]["data"] = this.bridge.IsTargetAvailable
                ? GetCurve(this.bridge.TargetResult, this.bridge.RandomTargetResult)
                : null;
            o["result"]["ltass"]["data"] = GetCurve(this.bridge.LTASS, this.bridge.RandomLTASS);
            o["result"]["percentile99"]["data"] = GetCurve(this.bridge.Percentile99, this.bridge.RandomPercentile99);
            o["result"]["percentile30"]["data"] = GetCurve(this.bridge.Percentile30, this.bridge.RandomPercentile30);
            o["result"]["unaidedsii"]["data"] = this.bridge.UnaidedSII;
            o["result"]["aidedsii"]["data"] = this.bridge.AidedSII;
            return o;
        }
    }

    internal class GetTestCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("getTest"))
            {
                return ClfsServer.ServiceLocator.Resolve<GetTestCommand>();
            }
            return null;
        }
    }
}
