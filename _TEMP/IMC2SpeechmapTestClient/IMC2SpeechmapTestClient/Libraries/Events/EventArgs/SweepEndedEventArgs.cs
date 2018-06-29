using IMC2SpeechmapTestClient.Libraries.IMC.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class SweepEndedEventArgs : System.EventArgs
    {
        public SweepEndedData Data { get; set; }

        public SweepEndedEventArgs(SweepEndedData sweepEndedData)
        {
            Data = sweepEndedData;
        }
    }
}
