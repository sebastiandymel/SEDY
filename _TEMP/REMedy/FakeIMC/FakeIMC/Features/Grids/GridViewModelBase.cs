using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using FakeIMC.Math;

namespace FakeIMC.UI
{
    public class GridViewModelBase
    {
        private readonly IImcGridModel model;

        public GridViewModelBase(IImcGridModel model)
        {
            this.model = model;
        }

        private int updateInProgress;

        protected void Update(ObservableCollection<FreqValCell> target, Spectrum input)
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

        protected void Subscribe(ObservableCollection<FreqValCell> source, CurveType type, CurveLevel? level = null)
        {
            var observables = source.Select(o => o.OnAnyPropertyChanges(c => c.Value));
            var merged = Observable.Merge(observables).Subscribe(arg =>
            {
                if (this.updateInProgress == 0)
                {
                    Debug.WriteLine($"Curve {type} updated: Freq={arg.Frequency}, Value={arg.NumberValue}");
                    this.model.UpdateCurveValue(type, level, arg.Frequency, arg.NumberValue);
                }
            });
        }

        protected static ObservableCollection<FreqValCell> GetDefaultCells()
        {
            return new ObservableCollection<FreqValCell>
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
        }
    }
}