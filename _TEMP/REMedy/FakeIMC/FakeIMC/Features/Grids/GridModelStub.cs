using System;
using System.Reactive.Subjects;
using FakeIMC.Math;

namespace FakeIMC.UI
{
    public class GridModelStub : IImcGridModel
    {
        public void UpdateCurveValue(CurveType type, CurveLevel? level, double freq, double val)
        {
        }

        public void UpdateSiiValue(MeasurementType type, double value)
        {
        }

        public void SetSpeechMode(bool isSpeechMode)
        {
        }

        public void ExportToXml()
        {
        }

        public void ImportFromXml()
        {
        }

        public IObservable<Spectrum> HighInputChanged { get; } = new ReplaySubject<Spectrum>();
        public IObservable<Spectrum> LowInputChanged { get; } = new ReplaySubject<Spectrum>();
        public IObservable<Spectrum> MediumInputChanged { get; } = new ReplaySubject<Spectrum>();
        public IObservable<Spectrum> ReugChanged { get; } = new ReplaySubject<Spectrum>();
        public IObservable<Spectrum> Percentiles30Changed { get; } = new ReplaySubject<Spectrum>();
        public IObservable<Spectrum> Percentiles99Changed { get; } = new ReplaySubject<Spectrum>();
        public IObservable<Spectrum> LtassChanged { get; } = new ReplaySubject<Spectrum>();

    }
}