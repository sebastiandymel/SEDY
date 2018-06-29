using IMC2SpeechmapTestClient.Libraries.OfficeSystem;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class OfficeSystemConnectedEventArgs : System.EventArgs
    {
        public OfficeSystemData Data { get; set; }

        public OfficeSystemConnectedEventArgs(OfficeSystemData officeSystemData)
        {
            Data = officeSystemData;
        }
    }
}
