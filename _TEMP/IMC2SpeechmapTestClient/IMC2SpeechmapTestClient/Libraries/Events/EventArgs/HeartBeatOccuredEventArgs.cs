using IMC2SpeechmapTestClient.Libraries.IMC.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class HeartBeatOccuredEventArgs : System.EventArgs
    {
        public HeartBeatData Data { get; set; }

        public HeartBeatOccuredEventArgs(HeartBeatData heartBeatData)
        {
            Data = heartBeatData;
        }
    }
}
