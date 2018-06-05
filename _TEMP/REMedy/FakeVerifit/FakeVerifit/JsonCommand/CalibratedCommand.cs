using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    class CalibratedCommand : IFakeVerifitCommand
    {
        private IUiBridge bridge;

        public CalibratedCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }
        public IsCalibratedData Data { get; set; }
        public JObject GetJsonResponse()
        {
            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            var isThisSideCalibrated = (Data.Side == Side.Left && this.bridge.IsLeftSideCalibrated) ||
                                       (Data.Side == Side.Right && this.bridge.IsRightSideCalibrated);
            if (!isThisSideCalibrated)
            {
                jobject["result"] = "0";
            }

            jobject["id"] = "calibrated";
            return jobject;
        }
    }

    internal class CalibratedCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("calibrated"))
            {
                VerifitParser parser = new VerifitParser();
                var data = parser.ParseIsCalibratedCommand(command);
                var cmd =  ClfsServer.ServiceLocator.Resolve<CalibratedCommand>();
                cmd.Data = data;
                return cmd;
            }
            return null;
        }
    }
}
