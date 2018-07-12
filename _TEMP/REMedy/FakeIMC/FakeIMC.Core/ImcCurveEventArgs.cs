using System;
using FakeIMC.Math;

namespace FakeIMC.Core
{
    public class ImcCurveEventArgs : EventArgs
    {
        public Spectrum Low { get; set; }
        public Spectrum Medium { get; set; }
        public Spectrum High { get; set; }
        public Spectrum Reug { get; set; }
        public Spectrum Perc30 { get; set; }
        public Spectrum Perc99 { get; set; }
        public Spectrum Ltass { get; set; }
        public bool AddReugToReag { get; set; }
        public bool AddRandomValues { get; set; }
        public double SpeechLowOffset { get; set; }
        public double SpeechHighOffset { get; set; }
        public double SiiAided { get; set; }
        public double SiiUnaided { get; set; }
    }
}