using System.Collections.Generic;

namespace FakeVerifit
{
    public class UiBridge : IUiBridge
    {
        public IEnumerable<string> Targets { get; set; }
        public IEnumerable<string> Stimuli { get; set; }
        public IEnumerable<string> Levels { get; set; }
        public int MeasurementTime { get; set; }
        public int EqualizationTime { get; set; }
        public IEnumerable<string> TargetResult { get; set; }
    }
}