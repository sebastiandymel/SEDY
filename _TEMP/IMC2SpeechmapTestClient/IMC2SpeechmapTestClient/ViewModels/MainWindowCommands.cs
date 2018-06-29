using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace IMC2SpeechmapTestClient.ViewModels
{
    public class MainWindowCommands
    {
        public ICommand Quit { get; set; }
        public ICommand SetRunModeToStandalone { get; set; }
        public ICommand SetRunModeToNoah { get; set; }
        public ICommand RegisterMyselfInCurrentMode { get; set; }
        public ICommand SetRunModeToNone { get; set; }

        public ICommand LaunchRemModule { get; set; }

        public ICommand CloseRemModule { get; set; }

        public ICommand ClearLogs { get; set; }

        public ICommand NullCommand { get; set; } = new RelayCommand(() => { }, () => false);
    }
}
