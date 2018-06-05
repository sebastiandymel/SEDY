using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Remedy.CommonUI
{
    public static class FreqValExtensions
    {
        public static IEnumerable<FreqVal> Interpolate(this IEnumerable<FreqVal> originalPoints,
            int[] toIntrapolate)
        {
            var points = originalPoints.ToList();
            var interpolatedPoints = new List<FreqVal>();
            foreach (var ip in toIntrapolate)
            {
                var exactPoint = points.FirstOrDefault(x => x.Frequency == ip);
                if (exactPoint != null)
                {
                    interpolatedPoints.Add(new FreqVal
                    {
                        Frequency = exactPoint.Frequency,
                        Value = exactPoint.Value
                    });
                    continue;
                }

                var startPoint = points.LastOrDefault(p => p.Frequency < ip);
                if (startPoint == null)
                {
                    startPoint = points.First();
                    interpolatedPoints.Add(new FreqVal
                    {
                        Frequency = ip,
                        Value = startPoint.Value
                    });
                    continue;
                }

                var endPoint = points.FirstOrDefault(p => p.Frequency > ip);
                if (endPoint == null)
                {
                    endPoint = points.Last();
                    interpolatedPoints.Add(new FreqVal
                    {
                        Frequency = ip,
                        Value = endPoint.Value
                    });
                    continue;
                }

                var gap = Math.Abs(startPoint.Frequency - endPoint.Frequency);
                var firstGap = Math.Abs(startPoint.Frequency - ip);
                var lastGap = Math.Abs(ip - endPoint.Frequency);

                double coff1 = 1 - ((double)firstGap / (double)gap);
                coff1 = coff1 * startPoint.NumberValue;
                double coff2 = 1 - ((double)lastGap / (double)gap);
                coff2 = coff2 * endPoint.NumberValue;
                double value = coff1 + coff2;
                interpolatedPoints.Add(new FreqVal
                {
                    Frequency = ip,
                    Value = string.Format(CultureInfo.InvariantCulture,"{0:0.00}", value)
                });
            }

            return interpolatedPoints;
        }
    }
}