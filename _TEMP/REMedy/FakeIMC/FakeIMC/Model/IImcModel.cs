using System;
using System.Collections.Generic;
using FakeIMC.Math;

namespace FakeIMC
{
    public interface IImcModel
    {
        void Start();
        void RegisterNoah();
        void UnRegisterNoah();
        void RegisterLocal();
        void UnRegisterLocal();
        void ConnectToNoah();

        [DispatchToUi]
        void ExportToXml();
        [DispatchToUi]
        void ImportFromXml();

        void SetHeartBeat(bool show);
        void SendError(string error);
        void UpdateCurveValue(CurveType type, double freq, double val);
        void UpdateCurvesConfiguration(CurvesConfiguration config);
        IEnumerable<string> PossibleErrors { get; }
        IObservable<LogItem> DataChanged { get; }
        IObservable<Spectrum> HighInputChanged { get; }
        IObservable<Spectrum> LowInputChanged { get; }
        IObservable<Spectrum> MediumInputChanged { get; }
        IObservable<Spectrum> ReugChanged { get; }
        IObservable<Spectrum> Percentiles30Changed 
        {
            get;
        }
        IObservable<Spectrum> Percentiles99Changed
        {
            get;
        }
        IObservable<Spectrum> LtassChanged
        {
            get;
        }
    }
}