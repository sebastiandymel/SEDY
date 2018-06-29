using IMC2SpeechmapTestClient.Libraries.OfficeSystem;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class OfficeSystemDisconnectedEventArgs : System.EventArgs
    {
        public OfficeSystemData Data { get; set; }

        public OfficeSystemDisconnectedEventArgs(OfficeSystemData officeSystemData)
        {
            Data = officeSystemData;
        }
    }
}
