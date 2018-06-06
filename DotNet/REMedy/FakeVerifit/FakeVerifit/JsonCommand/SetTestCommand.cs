using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    class SetTestCommand : IFakeVerifitCommand
    {
        private readonly IMeasurementService measurementService;
        private readonly IUiBridge bridge;
        public SetTestData Data { get; set; }

        public SetTestCommand(IUiBridge bridge, IMeasurementService measurementService)
        {
            this.bridge = bridge;
            this.measurementService = measurementService;
        }

        public JObject GetJsonResponse()
        {
            this.measurementService.StartMeasurement();
            UpdateSlotInformation(Data.SlotNumber, Data.Level);
            var jobject = JObject.Parse(EmbededResource.GetFileText("success.json"));
            jobject["id"] = "setTest";
            return jobject;
        }

        private void UpdateSlotInformation(string slotNumber, string inputLevel)
        {
            var slotInformation = new VerifitDataItem(inputLevel, inputLevel);
            switch (slotNumber)
            {
                case "1":
                    this.bridge.Slot1 = slotInformation;
                    return;
                case "2":
                    this.bridge.Slot2 = slotInformation;
                    return;
                case "3":
                    this.bridge.Slot3 = slotInformation;
                    return;
                case "4":
                    this.bridge.Slot4 = slotInformation;
                    return;
            }
        }
    }

    internal class SetTestCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string line)
        {
            if (line.StartsWith("setTest"))
            {
                VerifitParser parser = new VerifitParser();
                var command =  ClfsServer.ServiceLocator.Resolve<SetTestCommand>();
                command.Data = parser.ParseSetTestCommand(line);
                return command;
            }
            return null;
        }
    }

}
