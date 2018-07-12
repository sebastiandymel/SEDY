using System;

namespace FakeIMC
{
    public interface ICurveConfigurator
    {
        void UpdateCurvesConfiguration(CurvesConfiguration config);
        IObservable<CurvesConfiguration> CurvesConfigurationChanged { get; }
    }
}