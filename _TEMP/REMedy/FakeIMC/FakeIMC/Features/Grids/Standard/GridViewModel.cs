using System.Collections.ObjectModel;
using Reactive.Bindings;
using System.Reactive.Linq;
using System;

namespace FakeIMC.UI
{
    public class GridViewModel: GridViewModelBase
    {
        private readonly IImcGridModel model;

        public GridViewModel(IImcGridModel model): base(model)
        {
            this.model = model;

            const int throttle = 200;
            this.model.LowInputChanged.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(Low, s); });

            this.model.MediumInputChanged.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(Medium, s); });

            this.model.HighInputChanged.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(High, s); });

            this.model.ReugChanged.Throttle(TimeSpan.FromMilliseconds(throttle))
                .ForEachAsync(s => { Update(REUG, s); });


            Subscribe(Low, CurveType.Output, CurveLevel.Low);
            Subscribe(Medium, CurveType.Output, CurveLevel.Medium);
            Subscribe(High, CurveType.Output, CurveLevel.High);
            Subscribe(REUG, CurveType.Reug);
        }

        public ObservableCollection<FreqValCell> Low { get; } = GetDefaultCells();
        public ObservableCollection<FreqValCell> Medium { get; } = GetDefaultCells();
        public ObservableCollection<FreqValCell> High { get; } = GetDefaultCells();
        public ObservableCollection<FreqValCell> REUG { get; } = GetDefaultCells();
    }
}