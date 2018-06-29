namespace IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes
{
    public class HearingSoftwareModule
    {
        public int ModuleId { get; set; }

        public string ModulePrintName { get; set; }

        public string ModuleName { get; set; }

        public string ImcServerName { get; set; }

        public HearingSoftwareProtocol[] Protocols { get; set; }

        public string ExecutablePath { get; set; }

        public string CommandLineArgs { get; set; }
    }
}
