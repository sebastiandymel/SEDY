using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FakeIMC.Core;
using FakeIMC.Math;

namespace FakeIMC
{
    public class ImcModel : IImcModel
    {
        private readonly FakeImcCore core;

        public ImcModel()
        {
            DataChanged = new ReplaySubject<LogItem>();
            this.core = new FakeImcCore();
            this.core.ImcEntryLog += (s, e) =>
            {
                ((ReplaySubject<LogItem>)DataChanged).OnNext(new LogItem
                {
                    Text = e.Message,
                    Severity = Severity.Normal
                });
            };
            this.core.Failed += (s, e) =>
            {
                ((ReplaySubject<LogItem>)DataChanged).OnNext(new LogItem
                {
                    Text = e.Message,
                    Severity = Severity.Error
                });
            };
            this.core.Warning += (s, e) =>
            {
                ((ReplaySubject<LogItem>)DataChanged).OnNext(new LogItem
                {
                    Text = e.Message,
                    Severity = Severity.Warning
                });
            };


            HighInputChanged = new ReplaySubject<Spectrum>();
            LowInputChanged  = new ReplaySubject<Spectrum>();
            MediumInputChanged = new ReplaySubject<Spectrum>();
            ReugChanged = new ReplaySubject<Spectrum>();
            LtassChanged = new ReplaySubject<Spectrum>();
            Percentiles30Changed = new ReplaySubject<Spectrum>();
            Percentiles99Changed = new ReplaySubject<Spectrum>();

            this.core.CurvesChanged += (s, e) =>
            {
                ((ReplaySubject<Spectrum>)HighInputChanged).OnNext(e.High);
                ((ReplaySubject<Spectrum>)LowInputChanged).OnNext(e.Low);
                ((ReplaySubject<Spectrum>)MediumInputChanged).OnNext(e.Medium);
                ((ReplaySubject<Spectrum>)ReugChanged).OnNext(e.Reug);
                ((ReplaySubject<Spectrum>)Percentiles30Changed).OnNext(e.Perc30);
                ((ReplaySubject<Spectrum>)Percentiles99Changed).OnNext(e.Perc99);
                ((ReplaySubject<Spectrum>)LtassChanged).OnNext(e.Ltass);
            };
        }

        public void Start()
        {
            this.core.Start();
        }

        public void RegisterNoah()
        {
            this.core.RegisterNoah();
        }

        public void UnRegisterNoah()
        {
            this.core.UnRegisterNoah();
        }

        public void RegisterLocal()
        {
            this.core.RegisterLocal();
        }

        public void UnRegisterLocal()
        {
            this.core.UnregisterLocal();
        }

        public void ConnectToNoah()
        {
            this.core.ConnectToNoah();
        }

        public void ExportToXml()
        {
            this.core.SaveToXml();
        }

        public void ImportFromXml()
        {
            this.core.LoadFromXml();
        }

        public void SetHeartBeat(bool show)
        {
            this.core.ShowHeartbeat(show);
        }


        public void SendError(string error)
        {
            this.core.SendError(error);
        }

        public void UpdateCurveValue(CurveType type, double freq, double val)
        {
            switch (type)
            {
                case CurveType.Low:
                    this.core.SetLowCurveValue(freq, val);
                    break;
                case CurveType.Medium:
                    this.core.SetMediumCurveValue(freq, val);
                    break;
                case CurveType.High:
                    this.core.SetHighCurveValue(freq, val);
                    break;
                case CurveType.Reug:
                    this.core.SetReugCurveValue(freq, val);
                    break;
                case CurveType.Percentiles30:
                    this.core.SetPercentiles30Value(freq, val);
                    break;
                case CurveType.Percentiles99:
                    this.core.SetPercentiles99Value(freq, val);
                    break;
                case CurveType.Ltass:
                    this.core.SetLtassValue(freq, val);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void UpdateCurvesConfiguration(CurvesConfiguration config)
        {
            this.core.ConfigureCurves(config.AddRandomValues, config.AddReugToReag);
        }

        public IEnumerable<string> PossibleErrors => this.core.PossibleErrors;

        public IObservable<LogItem> DataChanged { get; }

        public IObservable<Spectrum> HighInputChanged { get; }

        public IObservable<Spectrum> LowInputChanged { get; }

        public IObservable<Spectrum> MediumInputChanged { get; }

        public IObservable<Spectrum> ReugChanged { get; }

        public IObservable<Spectrum> Percentiles30Changed
        {
            get;
        }

        public IObservable<Spectrum> Percentiles99Changed
        {
            get;
        }

        public IObservable<Spectrum> LtassChanged
        {
            get;
        }
    }
}
