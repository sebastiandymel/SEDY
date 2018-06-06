using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    internal class CalcTargetsCommand : CurveCommandBase, IFakeVerifitCommand
    {
        public CalcTargetsCommand(IUiBridge bridge) : base(bridge)
        {
            this.bridge = bridge;
        }

        public JObject GetJsonResponse()
        {
            if (this.bridge.IsTargetAvailable)
            {
                var baseObject = JObject.Parse(EmbededResource.GetFileText("calcTargets.json"));
                baseObject["result"]["targets"]["data"] = GetCurve(this.bridge.TargetResult, this.bridge.RandomTargetResult);
                return baseObject;
            }
            return ErrorHelper.CreateErrorResponse(ErrorEnum.NoTargetsForStimulus);
        }
    }

    internal class CalcTargetsCommandCreator : IFakeVerifitCommandCreator
    {
        public IFakeVerifitCommand Create(string command)
        {
            if (command.StartsWith("calcTargets"))
            {
                return ClfsServer.ServiceLocator.Resolve<CalcTargetsCommand>();
            }
            return null;
        }
    }
}
