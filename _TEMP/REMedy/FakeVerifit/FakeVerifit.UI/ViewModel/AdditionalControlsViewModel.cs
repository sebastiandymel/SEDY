using System;
using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Remedy.CommonUI;
using Soltys.ChangeCase;
using SuperSocket.SocketBase;
namespace FakeVerifit.UI
{
    public class AdditionalControlsViewModel : ViewModelBase
    {
        private ILifetimeScope scope;

        public ObservableCollection<Setting> DropReasons { get; } = new ObservableCollection<Setting>();
        public RelayCommand DropUsersCommand { get; }

        public ObservableCollection<Setting> VerifitErrors { get; } = new ObservableCollection<Setting>();

        public AdditionalControlsViewModel(ILifetimeScope scope)
        {
            this.scope = scope;
            var names = Enum.GetNames(typeof(CloseReason));
            foreach (var name in names)
            {
                DropReasons.Add(new Setting
                {
                    Value = name,
                    LocalizedName = name.SentenceCase().UpperCaseFirst(),
                    GroupName = "DropReasons"
                });
            }
            DropReasons[0].IsChecked = true;
            DropUsersCommand = new RelayCommand(DropUsers);

            var verifitErros = (ErrorEnum[])Enum.GetValues(typeof(ErrorEnum));
            VerifitErrors.Add(new Setting
            {
                GroupName = "VerifitErrors",
                Value = null,
                LocalizedName = "Do not send error",
                IsChecked = true
            });
            foreach (var error in verifitErros)
            {
                VerifitErrors.Add(new Setting
                {
                    GroupName = "VerifitErrors",
                    Value = error.ToErrorCode(),
                    LocalizedName = error.ToString().SentenceCase().UpperCaseFirst()
                });
            }
        }

        private void DropUsers()
        {
            var bridge = this.scope.Resolve<IUiBridge>();
            bridge.ClearSetupData();

            //Resolving ClfsServer by LifetimeScope cause adding IClfsServer to ctor causes StackOverflow 
            var closeReason = (CloseReason)Enum.Parse(typeof(CloseReason), SelectedDropReason.Value);
            var clfsServer = this.scope.Resolve<IClfsServer>();
            clfsServer.CloseAllConnections(closeReason);
        }
        public Setting SelectedDropReason
        {
            get { return DropReasons.FirstOrDefault(x => x.IsChecked); }
        }

        public Setting SelectedVerifitError
        {
            get { return VerifitErrors.FirstOrDefault(x => x.IsChecked); }
        }

        #region ViewModel Properties
        
        private bool startReturnBabble;
        public bool StartReturnBabble
        {
            get => this.startReturnBabble;
            set => Set(ref this.startReturnBabble, value);
        }

        private bool haltServerResponse;
        public bool HaltServerResponse
        {
            get => this.haltServerResponse;
            set => Set(ref this.haltServerResponse, value);
        }

        private bool incompatibleVersion;
        public bool IncompatibleVersion
        {
            get => this.incompatibleVersion;
            set => Set(ref this.incompatibleVersion, value);
        }

        private bool isLeftSideCalibrated = true;
        public bool IsLeftSideCalibrated
        {
            get => this.isLeftSideCalibrated;
            set => Set(ref this.isLeftSideCalibrated, value);
        }

        private bool isRightSideCalibrated = true;
        public bool IsRightSideCalibrated
        {
            get => this.isRightSideCalibrated;
            set => Set(ref this.isRightSideCalibrated, value);
        }

        private bool isTargetAvailable = true;
        public bool IsTargetAvailable
        {
            get => this.isTargetAvailable;
            set => Set(ref this.isTargetAvailable, value);
        }
        
        private string slowModeTime = "0";
        public string SlowModeTime
        {
            get => this.slowModeTime;
            set => Set(ref this.slowModeTime, value);
        }
        #endregion ViewModel Properties
    }
}
