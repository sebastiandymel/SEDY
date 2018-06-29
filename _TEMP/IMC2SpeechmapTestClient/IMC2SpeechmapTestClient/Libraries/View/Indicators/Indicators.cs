using GalaSoft.MvvmLight;

namespace IMC2SpeechmapTestClient.Libraries.View
{
    public class Indicators: ViewModelBase
    {
        private ControlState isRunModeSelected;
        public ControlState IsRunModeSelected
        {
            get => this.isRunModeSelected;
            set
            {
                this.isRunModeSelected = value;
                RaisePropertyChanged(nameof(Indicators.IsRunModeSelected));
            }
        }


        private ControlState isModuleConnected;
        public ControlState IsModuleConnected
        {
            get => this.isModuleConnected;
            set
            {
                this.isModuleConnected = value;
                RaisePropertyChanged(nameof(Indicators.IsModuleConnected));
            }
        }


        private ControlState didHeartBeatOccur;
        public ControlState DidHeartBeatOccur
        {
            get => this.didHeartBeatOccur;
            set
            {
                this.didHeartBeatOccur = value;
                RaisePropertyChanged(nameof(Indicators.DidHeartBeatOccur));
            }
        }


        private ControlState wasProtololNoSet;
        public ControlState WasProtololNoSet
        {
            get => this.wasProtololNoSet;
            set
            {
                this.wasProtololNoSet = value;
                RaisePropertyChanged(nameof(Indicators.WasProtololNoSet));
            }
        }


        private ControlState wasPrepareSystemCommandSent;
        public ControlState WasPrepareSystemCommandSent
        {
            get => this.wasPrepareSystemCommandSent;
            set
            {
                this.wasPrepareSystemCommandSent = value;
                RaisePropertyChanged(nameof(Indicators.WasPrepareSystemCommandSent));
            }
        }

        private ControlState wasSystemPrepared;
        public ControlState WasSystemPrepared
        {
            get => this.wasSystemPrepared;
            set
            {
                this.wasSystemPrepared = value;
                RaisePropertyChanged(nameof(Indicators.WasSystemPrepared));
            }
        }


        private ControlState wasModuleShowed;
        public ControlState WasModuleShowed
        {
            get => this.wasModuleShowed;
            set
            {
                this.wasModuleShowed = value;
                RaisePropertyChanged(nameof(Indicators.WasModuleShowed));
            }
        }


        private ControlState wasProbeTubeCalibrationCommandSent;
        public ControlState WasProbeTubeCalibrationCommandSent
        {
            get => this.wasProbeTubeCalibrationCommandSent;
            set
            {
                this.wasProbeTubeCalibrationCommandSent = value;
                RaisePropertyChanged(nameof(Indicators.WasProbeTubeCalibrationCommandSent));
            }
        }


        private ControlState wasProbeTubeCalibrationPerformed;
        public ControlState WasProbeTubeCalibrationPerformed
        {
            get => this.wasProbeTubeCalibrationPerformed;
            set
            {
                this.wasProbeTubeCalibrationPerformed = value;
                RaisePropertyChanged(nameof(Indicators.WasProbeTubeCalibrationPerformed));
            }
        }


        private ControlState wasRearMeasurementCommandSent;
        public ControlState WasRearMeasurementCommandSent
        {
            get => this.wasRearMeasurementCommandSent;
            set
            {
                this.wasRearMeasurementCommandSent = value;
                RaisePropertyChanged(nameof(Indicators.WasRearMeasurementCommandSent));
            }
        }


        private ControlState wasRearMeasurementPerformed;
        public ControlState WasRearMeasurementPerformed
        {
            get => this.wasRearMeasurementPerformed;
            set
            {
                this.wasRearMeasurementPerformed = value;
                RaisePropertyChanged(nameof(Indicators.WasRearMeasurementPerformed));
            }
        }


        private ControlState wasLeftAidedSiiReceived;
        public ControlState WasLeftAidedSiiReceived
        {
            get => this.wasLeftAidedSiiReceived;
            set
            {
                this.wasLeftAidedSiiReceived = value;
                RaisePropertyChanged(nameof(Indicators.WasLeftAidedSiiReceived));
            }
        }


        private ControlState wasLeftUnaidedSiiReceived;
        public ControlState WasLeftUnaidedSiiReceived
        {
            get => this.wasLeftUnaidedSiiReceived;
            set
            {
                this.wasLeftUnaidedSiiReceived = value;
                RaisePropertyChanged(nameof(Indicators.WasLeftUnaidedSiiReceived));
            }
        }



        private ControlState wasRightAidedSiiReceived;
        public ControlState WasRightAidedSiiReceived
        {
            get => this.wasRightAidedSiiReceived;
            set
            {
                this.wasRightAidedSiiReceived = value;
                RaisePropertyChanged(nameof(Indicators.WasRightAidedSiiReceived));
            }
        }


        private ControlState wasRightUnaidedSiiReceived;
        public ControlState WasRightUnaidedSiiReceived
        {
            get => this.wasRightUnaidedSiiReceived;
            set
            {
                this.wasRightUnaidedSiiReceived = value;
                RaisePropertyChanged(nameof(Indicators.WasRightUnaidedSiiReceived));
            }
        }


        private ControlState wasPercentile1Received;
        public ControlState WasPercentile1Received
        {
            get => this.wasPercentile1Received;
            set
            {
                this.wasPercentile1Received = value;
                RaisePropertyChanged(nameof(Indicators.WasPercentile1Received));
            }
        }


        private ControlState wasPercentile2Received;
        public ControlState WasPercentile2Received
        {
            get => this.wasPercentile2Received;
            set
            {
                this.wasPercentile2Received = value;
                RaisePropertyChanged(nameof(Indicators.WasPercentile2Received));
            }
        }
    }
}
