using System.Globalization;
using GalaSoft.MvvmLight;
using Remedy.Core;

namespace Remedy.CommonUI
{
    public class FreqVal : ViewModelBase, IFreqVal
    {
        private double? numValue;
        private string val;
        public int Frequency { get; set; }

        public string Value
        {
            get { return this.val; }
            set
            {
                if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out double pointValue))
                {
                    Set(ref this.val, value);
                    this.numValue = pointValue;
                }
                else
                {
                    this.numValue = null;
                    Set(ref this.val, "");
                }
            }
        }

        public double NumberValue
        {
            get
            {
                var pointValue = double.NegativeInfinity;
                if (double.TryParse(Value, NumberStyles.Number, CultureInfo.InvariantCulture, out pointValue) || string.IsNullOrEmpty(Value))
                {
                    return pointValue;
                }
                return double.NegativeInfinity;
            }
        }

        public override string ToString()
        {
            var numberValueString = this.numValue == null ? "" : this.numValue.ToString();
            return "{" + Frequency + "," + numberValueString + "}";
        }
    }
}