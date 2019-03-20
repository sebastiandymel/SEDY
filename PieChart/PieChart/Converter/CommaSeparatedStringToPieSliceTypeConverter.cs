using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace PieChart
{
    public class CommaSeparatedStringToPieSliceTypeConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(IEnumerable);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
           if (value is IEnumerable<IPieSlice> splices)
            {
                return string.Join(",", splices.Select(x => x.Value));
            }
            return null;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new PieSlices(value as string);
        }
    }
}
