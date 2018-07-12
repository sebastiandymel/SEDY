using System;
using System.Reactive.Subjects;
using FakeIMC.Core;

namespace FakeIMC
{
    public class CurveConfigurator: ICurveConfigurator
    {
        private readonly FakeImcCore core;

        public CurveConfigurator(FakeImcCore core)
        {
            this.core = core;
            CurvesConfigurationChanged = new ReplaySubject<CurvesConfiguration>();
            this.core.CurvesChanged += (s, e) =>
            {
                ((ReplaySubject<CurvesConfiguration>) CurvesConfigurationChanged).OnNext(new CurvesConfiguration
                {
                    AddReugToReag = e.AddReugToReag,
                    AddRandomValues = e.AddRandomValues,
                    SpeechLowOffset = e.SpeechLowOffset,
                    SpeechHighOffset = e.SpeechHighOffset,
                    SiiAided = e.SiiAided,
                    SiiUnaided = e.SiiUnaided
                });
            };
        }

        public void UpdateCurvesConfiguration(CurvesConfiguration config)
        {
            this.core.ConfigureCurves(
                config.AddRandomValues,
                config.AddReugToReag,
                config.SpeechLowOffset,
                config.SpeechHighOffset);
        }

        public IObservable<CurvesConfiguration> CurvesConfigurationChanged { get; }
    }
}