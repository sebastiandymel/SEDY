using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Remedy.Core;

namespace FakeVerifit
{
    internal abstract class CurveCommandBase
    {
        protected IUiBridge bridge;

        protected CurveCommandBase(IUiBridge bridge)
        {
            this.bridge = bridge;
        }

        protected JToken GetCurve(IEnumerable<IFreqVal> curve, IEnumerable<IFreqVal> randomCurve)
        {
            if (this.bridge.RandomizeOutput)
            {
                return randomCurve.ConvertToJObject();
            }
            return curve.ConvertToJObject();
        }
    }
}