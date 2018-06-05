using System.Collections.Generic;
using Remedy.Core;

namespace FakeVerifit
{
    public interface IUiBridge
    {
        byte[] WindowScreenShot { get; }
        string LogoPath { get; }

        #region SettingsViewModel
        IEnumerable<IDataItem> Targets { get; }
        IEnumerable<IDataItem> Stimuli { get; }

        int MeasurementTime { get; }
        int EqualizationTime { get; }
        int UnaidedSII { get; }
        int AidedSII { get; }
        bool RandomizeOutput { get; }

        /// <summary>
        /// Returns value of "vf2" - Verifit2 or "vf1" - Verifit1
        /// </summary>
        string SelectedVerifitModel { get; }
        string VerifitDeviceFirmwareVersion { get; }
        IEnumerable<IFreqVal> TargetResult { get; }
        IEnumerable<IFreqVal> RandomTargetResult { get; }
        IEnumerable<IFreqVal> Percentile30 { get; }
        IEnumerable<IFreqVal> RandomPercentile30 { get; }
        IEnumerable<IFreqVal> Percentile99 { get; }
        IEnumerable<IFreqVal> RandomPercentile99 { get; }
        IEnumerable<IFreqVal> LTASS { get; }
        IEnumerable<IFreqVal> RandomLTASS { get; }
        IEnumerable<IFreqVal> SPLThresholds { get; }
        IEnumerable<IFreqVal> RandomSPLThresholds { get; }
        IEnumerable<IFreqVal> UCL { get; }
        IEnumerable<IFreqVal> RandomUCL { get; }
        IEnumerable<IFreqVal> RandomRecd { get; }
        #endregion
        #region Device ViewModel
        IDataItem TargetName { set; }
        IDataItem Language { set; }
        IDataItem LeftInstrument { set; }
        IDataItem RightInstrument { set; }
        IDataItem LeftVenting { set; }
        IDataItem RightVenting { set; }
        IDataItem Binaural { set; }
        IDataItem Age { set; }
        IDataItem Transducer { set; }
        //IDataItem LeftRecdCoupling { set; }
        //IDataItem RightRecdCoupling { set; }
        IDataItem LeftHl { set; }
        IDataItem RightHl { set; }
        IDataItem RightUcl { set; }
        IDataItem LeftUcl { set; }
        IDataItem RightBc { set; }
        IDataItem LeftBc { set; }
        //IDataItem LeftUseMeasuredRECD { set; }
        //IDataItem RightUseMeasuredRECD { set; }

        IDataItem Slot1 { set; }
        IDataItem Slot2 { set; }
        IDataItem Slot3 { set; }
        IDataItem Slot4 { set; }

        void ClearSetupData();
        #endregion

        #region Additional Controls
        IDataItem SelectedVerifitError { get; }
        bool HaltServerResponse { get; }
        bool StartReturningBabble { get; }
        bool IncompatibleVersion { get; }
        bool IsRightSideCalibrated { get; }
        bool IsLeftSideCalibrated { get; }
        bool IsTargetAvailable { get; }
        int SlowModeTime { get; }

        #endregion Additional Controls
    }
}