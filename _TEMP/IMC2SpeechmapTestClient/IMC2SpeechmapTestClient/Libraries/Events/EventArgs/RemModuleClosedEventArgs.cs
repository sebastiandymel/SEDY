using IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class RemModuleClosedEventArgs : System.EventArgs
    {
        public HearingSoftwareModule Data { get; set; }

        public RemModuleClosedEventArgs(HearingSoftwareModule hearingSoftwareModule)
        {
            Data = hearingSoftwareModule;
        }
    }
}
