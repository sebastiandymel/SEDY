using IMC2SpeechmapTestClient.Libraries.IMC.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class ProbeTubeCalibrationEndedEventArgs : System.EventArgs
    {
        public ProbetubeCalibrationEndedData Data { get; set; }

        public ProbeTubeCalibrationEndedEventArgs(ProbetubeCalibrationEndedData probetubeCalibrationEndedData)
        {
            Data = probetubeCalibrationEndedData;
        }
    }
}
