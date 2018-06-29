using IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class RemModuleLaunchedEventArgs : System.EventArgs
    {
        public HearingSoftwareModule Data { get; set; }

        public RemModuleLaunchedEventArgs(HearingSoftwareModule hearingSoftwareModule)
        {
            Data = hearingSoftwareModule;
        }
    }
}
