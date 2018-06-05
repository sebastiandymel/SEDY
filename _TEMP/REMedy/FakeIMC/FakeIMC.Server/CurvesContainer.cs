﻿using System;
using FakeIMC.Math;

namespace FakeIMC.Server
{
    public class CurvesContainer
    {
        public Spectrum CurveLowInput;
        public Spectrum CurveMediumInput;
        public Spectrum CurveHightInput;
        public Spectrum CurveREUG;
        public Spectrum Percentiles30;
        public Spectrum Percentiles99;
        public Spectrum Ltass;
        public bool AddRandomValues;
        public bool AddReug;
    }
}