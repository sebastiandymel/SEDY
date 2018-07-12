using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Reactive.Bindings;
using System;

namespace FakeIMC.UI
{
    public class SpeechMapGridViewModel: GridViewModelBase
    {
        public SpeechMapGridViewModel(IImcGridModel model, ICurveConfigurator configurator) : base(model)
        {
            const int throttle = 500;
            model.Percentiles30Changed.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(Percentiles30, s); });

            model.Percentiles99Changed.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(Percentiles99, s); });

            model.LtassChanged.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(Ltass, s); });

            Subscribe(Percentiles30, CurveType.Percentiles30, CurveLevel.Medium);
            Subscribe(Percentiles99, CurveType.Percentiles99, CurveLevel.Medium);
            Subscribe(Ltass, CurveType.Ltass, CurveLevel.Medium);

            SpeechViewOn = new ReactiveProperty<bool>();
            SpeechViewOn.Subscribe(model.SetSpeechMode);

            CurvesConfiguration current = null;
            
            LowOffset = new ReactiveProperty<double>(-30);
            LowOffset.Throttle(TimeSpan.FromMilliseconds(throttle)).Subscribe(x =>
            {
                if (current == null)
                {
                    return;
                }
                current.SpeechLowOffset = x;
                configurator.UpdateCurvesConfiguration(current);
            });

            HighOffset = new ReactiveProperty<double>(30);
            HighOffset.Throttle(TimeSpan.FromMilliseconds(throttle)).Subscribe(x =>
            {
                if (current == null)
                {
                    return;
                }
                current.SpeechHighOffset = x;
                configurator.UpdateCurvesConfiguration(current);
            });

            SiiAided = new ReactiveProperty<double>();
            SiiAided.Throttle(TimeSpan.FromMilliseconds(throttle)).Subscribe(x =>
            {
                if (current == null)
                {
                    return;
                }
                model.UpdateSiiValue(MeasurementType.Aided, x);
            });

            SiiUnaided = new ReactiveProperty<double>();
            SiiUnaided.Throttle(TimeSpan.FromMilliseconds(throttle)).Subscribe(x =>
            {
                if (current == null)
                {
                    return;
                }
                model.UpdateSiiValue(MeasurementType.Unaided, x);
            });

            configurator.CurvesConfigurationChanged.Throttle(TimeSpan.FromMilliseconds(throttle)).Subscribe(c =>
            {
                current = c;
                LowOffset.Value = c.SpeechLowOffset;
                HighOffset.Value = c.SpeechHighOffset;
                SiiAided.Value = c.SiiAided;
                SiiUnaided.Value = c.SiiUnaided;
            });
        }

        public ObservableCollection<FreqValCell> Percentiles30 { get; } = GetDefaultCells();
        public ObservableCollection<FreqValCell> Percentiles99 { get; } = GetDefaultCells();
        public ObservableCollection<FreqValCell> Ltass { get; } = GetDefaultCells();
        public ReactiveProperty<bool> SpeechViewOn { get; }
        public ReactiveProperty<double> LowOffset { get; }
        public ReactiveProperty<double> HighOffset { get; }
        public ReactiveProperty<double> SiiAided { get; }
        public ReactiveProperty<double> SiiUnaided { get; }

    }
}