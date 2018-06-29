using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace IMC2SpeechmapTestClient.Libraries.IMC
{
    public class ImcCommands : ViewModelBase
    {
        public RelayCommand SetProtocolNo { get; set; }

        public RelayCommand PrepareSystem { get; set; }

        public RelayCommand ShowModule { get; set; }

        public RelayCommand PerformProbetubeCalibration { get; set; }

        public RelayCommand PerformRearMeasurement { get; set; }
    }
}
