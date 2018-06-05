using System.Collections.ObjectModel;
using Reactive.Bindings;
using Remedy.CommonUI;
using System.Reactive.Linq;
using System.Linq;
using FakeIMC.Math;
using System;
using System.Diagnostics;
using System.Globalization;

namespace FakeIMC.UI
{
    public class GridViewModel
    {
        private int updateInProgress;

        private readonly IImcModel model;

        public GridViewModel(IImcModel model)
        {

            ExportToXml = new ReactiveCommand();
            ExportToXml.Subscribe(() => this.model.ExportToXml());

            ImportFromXml = new ReactiveCommand();
            ImportFromXml.Subscribe(() => this.model.ImportFromXml());
            this.model = model;

            const int throttle = 200;
            this.model.LowInputChanged.Throttle(System.TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(Low, s);
            });

            this.model.MediumInputChanged.Throttle(System.TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(Medium, s);
            });

            this.model.HighInputChanged.Throttle(System.TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(High, s);
            });

            this.model.ReugChanged.Throttle(TimeSpan.FromMilliseconds(throttle)).ForEachAsync(s =>
            {
                Update(REUG, s);
            });


            Subscribe(Low, CurveType.Low);
            Subscribe(Medium, CurveType.Medium);
            Subscribe(High, CurveType.High);
            Subscribe(REUG, CurveType.Reug);
        }

        public ReactiveCommand ImportFromXml { get; set; }

        public ReactiveCommand ExportToXml { get; set; }

        public ObservableCollection<FreqValCell> Low { get; } = new ObservableCollection<FreqValCell>
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

        public ObservableCollection<FreqValCell> Medium { get; } = new ObservableCollection<FreqValCell>
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

        public ObservableCollection<FreqValCell> High { get; } = new ObservableCollection<FreqValCell>
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
        
        public ObservableCollection<FreqValCell> REUG { get; } = new ObservableCollection<FreqValCell>
        {
            new FreqValCell {Frequency = 250, Value = "50"},
            new FreqValCell {Frequency = 500, Value = "50"},
            new FreqValCell {Frequency = 750, Value = "50"},
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