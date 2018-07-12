using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    public class SetClientCommand : IFakeVerifitCommand
    {
        private readonly IUiBridge bridge;

        public SetClientCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }

        public SetClientData Data { get; set; }

        public JObject GetJsonResponse()
        {
            this.bridge.TargetName = GetParameterValue("-target");
            this.bridge.Language = GetParameterValue("-languagetype");
            
            this.bridge.Age = GetParameterValue("-age");
            this.bridge.Transducer = GetParameterValue("-transducer");
            //this.bridge.LeftRecdCoupling = new VerifitDataItem(LeftRecdCoupling, LeftRecdCoupling);
            //this.bridge.RightRecdCoupling = new VerifitDataItem(RightRecdCoupling, RightRecdCoupling);
            this.bridge.LeftInstrument = GetParameterValue("-leftinstrument");
            this.bridge.RightInstrument = GetParameterValue("-rightinstrument");
            this.bridge.LeftVenting = GetParameterValue("-leftventing");
            this.bridge.RightVenting = GetParameterValue("-rightventing");
            this.bridge.Binaural = GetParameterValue("-binaural");
            this.bridge.LeftHl = GetParameterValue("-lefthl");
            this.bridge.RightHl = GetParameterValue("-righthl");
            this.bridge.RightUcl = GetParameterValue("-rightucl");
            this.bridge.LeftUcl = GetParameterValue("-leftucl");
            this.bridge.RightBc = GetParameterValue("-rightbc");
            this.bridge.LeftBc = GetParameterValue("-leftbc");

            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            jobject["id"] = "setClient";
            return jobject;
        }

        private VerifitDataItem GetParameterValue(string parameterKey)
        {
            if (!Data.Parameters.ContainsKey(parameterKey))
            {
                return new VerifitDataItem();
            }

            var value = Data.Parameters[parameterKey];
            if (value.StartsWith("{"))
            {
                return new VerifitDataItem(value, "[tooltip]");
            }
            return new VerifitDataItem(value, value);
        }

    }


    internal class SetClientCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string line)
        {
            if (line.StartsWith("setClient"))
            {
                VerifitParser verifitParser = new VerifitParser();
                var data = verifitParser.ParseSetClientCommand(line);
                var command = ClfsServer.ServiceLocator.Resolve<SetClientCommand>();
                command.Data = data;
                return command;
            }
            return null;
        }
    }
}
