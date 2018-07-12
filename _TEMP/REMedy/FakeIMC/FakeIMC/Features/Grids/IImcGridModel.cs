using System;
using FakeIMC.Math;

namespace FakeIMC
{
    public interface IImcGridModel
    {
        void UpdateCurveValue(CurveType type, CurveLevel? level, double freq, double val);
        void UpdateSiiValue(MeasurementType type, double value);
        void SetSpeechMode(bool isSpeechMode);
        IObservable<Spectrum> HighInputChanged { get; }
        IObservable<Spectrum> LowInputChanged { get; }
        IObservable<Spectrum> MediumInputChanged { get; }
        IObservable<Spectrum> ReugChanged { get; }
        IObservable<Spectrum> Percentiles30Changed { get; }
        IObservable<Spectrum> Percentiles99Changed { get; }
        IObservable<Spectrum> LtassChanged { get; }
        
    }
}