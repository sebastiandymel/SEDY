using System.Collections.ObjectModel;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Linq;
using FakeIMC.Math;
using System;
using System.Diagnostics;
using System.Globalization;

namespace FakeIMC.UI
{
    public class SpeechMapGridViewModel
    {
        private int updateInProgress;

        private readonly IImcModel model;

        public SpeechMapGridViewModel(IImcModel model)
        {
            this.model = model;

            const int throttle = 200;
            this.model.Percentiles30Changed.Throttle(System.TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(Percentiles30, s);
            });

            this.model.Percentiles99Changed.Throttle(System.TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(Percentiles99, s);
            });

            this.model.LtassChanged.Throttle(System.TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(Ltass, s);
            });

            Subscribe(Percentiles30, CurveType.Percentiles30);
            Subscribe(Percentiles99, CurveType.Percentiles99);
            Subscribe(Ltass, CurveType.Ltass);
        }

        public ReactiveCommand ImportFromXml
        {
            get; set;
        }

        public ReactiveCommand ExportToXml
        {
            get; set;
        }

        public ObservableCollection<FreqValCell> Percentiles30
        {
            get;
        } = new ObservableCollection<FreqValCell>
        {
            new FreqValCell {Frequency = 250, Value = "32.5"},
            new FreqValCell {Frequency = 500, Value = "42"},
            new FreqValCell {Frequency = 750, Value = "52"},
            new FreqValCell {Frequency = 1000, Value = "39"},
            new FreqValCell {Frequency = 1500, Value = "39"},
            new FreqValCell {Frequency = 2000, Value = "47"},
            new FreqValCell {Frequency = 3000, Value = "50"},
            new FreqValCell {Frequency = 4000, Value = "60"},
            new FreqValCell {Frequency = 6000, Value = "60"},
            new FreqValCell {Frequency = 8000, Value = "70"},
        };

        public ObservableCollection<FreqValCell> Percentiles99
        {
            get;
        } = new ObservableCollection<FreqValCell>
        {
            new FreqValCell {Frequency = 250, Value = "32.5"},
            new FreqValCell {Frequency = 500, Value = "42"},
            new FreqValCell {Frequency = 750, Value = "52"},
            new FreqValCell {Frequency = 1000, Value = "39"},
            new FreqValCell {Frequency = 1500, Value = "39"},
            new FreqValCell {Frequency = 2000, Value = "47"},
            new FreqValCell {Frequency = 3000, Value = "50"},
            new FreqValCell {Frequency = 4000, Value = "60"},
            new FreqValCell {Frequency = 6000, Value = "60"},
            new FreqValCell {Frequency = 8000, Value = "70"},
        };

        public ObservableCollection<FreqValCell> Ltass
        {
            get;
        } = new ObservableCollection<FreqValCell>
        {
            new FreqValCell {Frequency = 250, Value = "32.5"},
            new FreqValCell {Frequency = 500, Value = "42"},
            new FreqValCell {Frequency = 750, Value = "52"},
            new FreqValCell {Frequency = 1000, Value = "39"},
            new FreqValCell {Frequency = 1500, Value = "39"},
            new FreqValCell {Frequency = 2000, Value = "47"},
            new FreqValCell {Frequency = 3000, Value = "50"},
            new FreqValCell {Frequency = 4000, Value = "60"},
            new FreqValCell {Frequency = 6000, Value = "60"},
            new FreqValCell {Frequency = 8000, Value = "70"},
        };     

        private void Update(ObservableCollection<FreqValCell> target, Spectrum input)
        {
            if (input == null)
            {
                return;
            }
            this.updateInProgress++;
            for (int i = 0; i < input.Frequencies.Length; i++)
            {
                var freq = input.Frequencies[i];
                var toUpdate = target.FirstOrDefault(c => System.Math.Abs(c.Frequency - freq) < Double.Epsilon);
                if (toUpdate != null)
                {
                    toUpdate.Value = input.Values[i].ToString(CultureInfo.InvariantCulture);
                }
            }
            this.updateInProgress--;
        }

        private void Subscribe(ObservableCollection<FreqValCell> source, CurveType type)
        {
            var observables = source.Select(o => o.OnAnyPropertyChanges(c => c.Value));
            var merged = Observable.Merge(observables).Subscribe(arg =>
            {
                if (this.updateInProgress == 0)
                {
                    Debug.WriteLine($"Curve {type} updated: Freq={arg.Frequency}, Value={arg.NumberValue}");
                    this.model.UpdateCurveValue(type, arg.Frequency, arg.NumberValue);
                }
            });
        }
    }
}