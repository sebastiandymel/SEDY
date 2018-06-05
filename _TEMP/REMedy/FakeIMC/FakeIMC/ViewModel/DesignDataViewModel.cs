using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using FakeIMC.Math;

namespace FakeIMC.UI
{
    public class DesignDataViewModel: MainViewModel
    {
        public DesignDataViewModel() 
            : base(
                  new ModelStub(), 
                  new GridViewModel(new ModelStub()), 
                  new SpeechMapGridViewModel(new ModelStub()), 
                  null)
        {
        }

        private class ModelStub : IImcModel
        {
            public void Start()
            {
                
            }

            public void RegisterNoah()
            {
            }

            public void UnRegisterNoah()
            {
            }

            public void RegisterLocal()
            {
            }

            public void UnRegisterLocal()
            {
            }

            public void ConnectToNoah()
            {
            }

            public void ExportToXml()
            {
            }

            public void ImportFromXml()
            {
            }

            public void SetHeartBeat(bool show)
            {
            }

            public void SendError(string error)
            {
            }

            public void UpdateCurveValue(CurveType type, double freq, double val)
            {
            }

            public void UpdateCurvesConfiguration(CurvesConfiguration config)
            {
            }
            public IEnumerable<string> PossibleErrors { get; } = new List<string>();
            public IObservable<LogItem> DataChanged { get; } = new ReplaySubject<LogItem>();
            public IObservable<Spectrum> HighInputChanged { get; } = new ReplaySubject<Spectrum>();
            public IObservable<Spectrum> LowInputChanged { get; } = new ReplaySubject<Spectrum>();
            public IObservable<Spectrum> MediumInputChanged { get; } = new ReplaySubject<Spectrum>();
            public IObservable<Spectrum> ReugChanged { get; } = new ReplaySubject<Spectrum>();

            public IObservable<Spectrum> Percentiles30Changed { get; } = new ReplaySubject<Spectrum>();

            public IObservable<Spectrum> Percentiles99Changed { get; } = new ReplaySubject<Spectrum>();

            public IObservable<Spectrum> LtassChanged { get; } = new ReplaySubject<Spectrum>();
        }
    }
}