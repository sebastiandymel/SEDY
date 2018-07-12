using System;
using System.Reactive.Subjects;
using FakeIMC.Core;
using FakeIMC.Math;

namespace FakeIMC
{
    public class GridModel : IImcGridModel
    {
        private readonly FakeImcCore core;

        public GridModel(FakeImcCore core)
        {
            this.core = core;


            HighInputChanged = new ReplaySubject<Spectrum>();
            LowInputChanged = new ReplaySubject<Spectrum>();
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

        public void SetSpeechMode(bool isSpeechMode)
        {
            this.core.SetSpeechMode(isSpeechMode);
        }

        public void UpdateCurveValue(CurveType type, CurveLevel? level, double freq, double val)
        {
            switch (type)
            {
                case CurveType.Output:
                    if (level == CurveLevel.Low)
                    {
                        this.core.SetLowCurveValue(freq, val);
                    }
                    else if (level == CurveLevel.Medium)
                    {
                        this.core.SetMediumCurveValue(freq, val);
                    }
                    else if (level == CurveLevel.High)
                    {
                        this.core.SetHighCurveValue(freq, val);
                    }
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

        public void UpdateSiiValue(MeasurementType type, double value)
        {
            if (type == MeasurementType.Aided)
            {
                this.core.SetSiiAided(value);
            }
            else if (type == MeasurementType.Unaided)
            {
                this.core.SetSiiUnaided(value);
            }
        }

        public IObservable<Spectrum> HighInputChanged { get; }

        public IObservable<Spectrum> LowInputChanged { get; }

        public IObservable<Spectrum> MediumInputChanged { get; }

        public IObservable<Spectrum> ReugChanged { get; }

        public IObservable<Spectrum> Percentiles30Changed { get; }

        public IObservable<Spectrum> Percentiles99Changed { get; }

        public IObservable<Spectrum> LtassChanged { get; }
    }
}