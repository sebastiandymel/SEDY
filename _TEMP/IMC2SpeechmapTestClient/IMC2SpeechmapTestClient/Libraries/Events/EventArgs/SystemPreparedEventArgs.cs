using IMC2SpeechmapTestClient.Libraries.IMC.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class SystemPreparedEventArgs : System.EventArgs
    {
        public SystemPreparedData Data { get; set; }

        public SystemPreparedEventArgs(SystemPreparedData systemPreparedData)
        {
            Data = systemPreparedData;
        }
    }
}
