using System.Collections.Generic;
using System.Linq;
using Remedy.CommonUI;
using Remedy.Core;

namespace FakeVerifit.UI
{
    class UiBridge : IUiBridge
    {
        private readonly DeviceSetupViewModel deviceViewModel;
        private readonly SettingsViewModel settingsViewModel;
        private readonly MainViewModel mainViewModel;
        private readonly AdditionalControlsViewModel additionalControlsViewModel;

        public UiBridge(MainViewModel mainViewModel, SettingsViewModel settingsViewModel, DeviceSetupViewModel deviceViewModel, AdditionalControlsViewModel additionalControlsViewModel)
        {
            this.deviceViewModel = deviceViewModel;
            this.settingsViewModel = settingsViewModel;
            this.mainViewModel = mainViewModel;
            this.additionalControlsViewModel = additionalControlsViewModel;
        }

        public byte[] WindowScreenShot => this.mainViewModel.WindowsScreenShot;
        public string LogoPath => this.mainViewModel.LogoPath;

        #region Settings ViewModel
        public IEnumerable<IDataItem> Targets => this.settingsViewModel.Targets.Where(x => x.IsChecked);
        public IEnumerable<IDataItem> Stimuli => this.settingsViewModel.Stimuli.Where(x => x.IsChecked);
        public int MeasurementTime => ConvertToInt(this.settingsViewModel.MeasurementTime);
        public int EqualizationTime => ConvertToInt(this.settingsViewModel.EqualizationTime);
        public int UnaidedSII => ConvertToInt(this.settingsViewModel.UnaidedSII);
        public int AidedSII => ConvertToInt(this.settingsViewModel.AidedSII);

        public bool RandomizeOutput => this.settingsViewModel.RandomizeOutput;

        /// <summary>
        /// Returns value of "vf2" - Verifit2 or "vf1" - Verifit1
        /// </summary>
        public string SelectedVerifitModel
        {
            get
            {
                var selectedModel = this.settingsViewModel.SelectedVerifitModel.Value;
                if (selectedModel == UIBridgeConstants.Verifit1)
                {
                    return "vf1";
                }
                if (selectedModel == UIBridgeConstants.FakeVerifit)
                {
                    return "fv";
                }
                return "vf2";
            }
        }

        public string VerifitDeviceFirmwareVersion => this.settingsViewModel.VerifitFirmwareVersion;

        public IEnumerable<IFreqVal> TargetResult => this.settingsViewModel.TargetResult;
        public IEnumerable<IFreqVal> RandomTargetResult => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.TargetResult);
        public IEnumerable<IFreqVal> Percentile30 => this.settingsViewModel.Percentile30.Interpolate(this.settingsViewModel.PercentileFrequencyPoints);
        public IEnumerable<IFreqVal> RandomPercentile30 => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.Percentile30).Interpolate(this.settingsViewModel.PercentileFrequencyPoints);
        public IEnumerable<IFreqVal> Percentile99 => this.settingsViewModel.Percentile99.Interpolate(this.settingsViewModel.PercentileFrequencyPoints);
        public IEnumerable<IFreqVal> RandomPercentile99 => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.Percentile99).Interpolate(this.settingsViewModel.PercentileFrequencyPoints);
        public IEnumerable<IFreqVal> LTASS => this.settingsViewModel.LTASS.Interpolate(this.settingsViewModel.PercentileFrequencyPoints);
        public IEnumerable<IFreqVal> RandomLTASS => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.LTASS).Interpolate(this.settingsViewModel.PercentileFrequencyPoints);
        public IEnumerable<IFreqVal> SPLThresholds => this.settingsViewModel.SPLTresholds;
        public IEnumerable<IFreqVal> RandomSPLThresholds => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.SPLTresholds);
        public IEnumerable<IFreqVal> UCL => this.settingsViewModel.UCL;
        public IEnumerable<IFreqVal> RandomUCL => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.UCL);
        public IEnumerable<IFreqVal> RandomRecd => this.settingsViewModel.GenerateFreqValCollection(this.settingsViewModel.RECD);

        #endregion
        #region Device ViewModel
        public IDataItem TargetName { set => this.deviceViewModel.TargetName.Update(value); }
        public IDataItem Language { set => this.deviceViewModel.Language.Update(value); }
        public IDataItem LeftInstrument { set => this.deviceViewModel.LeftInstrument.Update(value); }
        public IDataItem RightInstrument { set => this.deviceViewModel.RightInstrument.Update(value); }
        public IDataItem LeftVenting { set => this.deviceViewModel.LeftVenting.Update(value); }
        public IDataItem RightVenting { set => this.deviceViewModel.RightVenting.Update(value); }
        public IDataItem Binaural { set => this.deviceViewModel.Binaural.Update(value); }
        public IDataItem Age { set => this.deviceViewModel.Age.Update(value); }
        public IDataItem Transducer { set => this.deviceViewModel.Transducer.Update(value); }
        //public IDataItem LeftRecdCoupling { set => this.deviceViewModel.LeftRecdCoupling.Update(value); }
        //public IDataItem RightRecdCoupling { set => this.deviceViewModel.RightRecdCoupling.Update(value); }
        public IDataItem LeftHl { set => this.deviceViewModel.LeftHl.Update(value); }
        public IDataItem RightHl { set => this.deviceViewModel.RightHl.Update(value); }
        public IDataItem RightUcl { set => this.deviceViewModel.RightUcl.Update(value); }
        public IDataItem LeftUcl { set => this.deviceViewModel.LeftUcl.Update(value); }
        public IDataItem RightBc { set => this.deviceViewModel.RightBc.Update(value); }
        public IDataItem LeftBc { set => this.deviceViewModel.LeftBc.Update(value); }
        //public IDataItem LeftUseMeasuredRECD { set => this.deviceViewModel.LeftUseMeasuredRECD.Update(value); }
        //public IDataItem RightUseMeasuredRECD { set => this.deviceViewModel.RightUseMeasuredRECD.Update(value); }
        public IDataItem Slot1 { set => this.deviceViewModel.Slot1.Update(value); }
        public IDataItem Slot2 { set => this.deviceViewModel.Slot2.Update(value); }
        public IDataItem Slot3 { set => this.deviceViewModel.Slot3.Update(value); }
        public IDataItem Slot4 { set => this.deviceViewModel.Slot4.Update(value); }
        public void ClearSetupData() => this.deviceViewModel.ClearDeviceSetup();

        #endregion

        #region Additional Controls
        public IDataItem SelectedVerifitError => this.additionalControlsViewModel.SelectedVerifitError;
        public bool HaltServerResponse => this.additionalControlsViewModel.HaltServerResponse;
        public bool StartReturningBabble => this.additionalControlsViewModel.StartReturnBabble;
        public bool IncompatibleVersion => this.additionalControlsViewModel.IncompatibleVersion;
        public bool IsRightSideCalibrated => this.additionalControlsViewModel.IsRightSideCalibrated;
        public bool IsLeftSideCalibrated => this.additionalControlsViewModel.IsLeftSideCalibrated;
        public bool IsTargetAvailable => this.additionalControlsViewModel.IsTargetAvailable;
        public int SlowModeTime => ConvertToInt(this.additionalControlsViewModel.SlowModeTime);

        #endregion Additional Controls
        private int ConvertToInt(string value)
        {
            var i = 0;
            if (int.TryParse(value, out i) || string.IsNullOrEmpty(value))
            {
                return i;
            }
            return default(int);
        }
    }
}
