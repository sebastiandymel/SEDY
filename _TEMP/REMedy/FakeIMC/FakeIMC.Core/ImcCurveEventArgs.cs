using System;
using FakeIMC.Math;

namespace FakeIMC.Core
{
    public class ImcCurveEventArgs : EventArgs
    {
        public Spectrum Low
        {
            get; set;
        }
        public Spectrum Medium
        {
            get; set;
        }
        public Spectrum High
        {
            get; set;
        }
        public Spectrum Reug
        {
            get; set;
        }
        public Spectrum Perc30
        {
            get; set;
        }
        public Spectrum Perc99
        {
            get; set;
        }
        public Spectrum Ltass
        {
            get; set;
        }
    }
}