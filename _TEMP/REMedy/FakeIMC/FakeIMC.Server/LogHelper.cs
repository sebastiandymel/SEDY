using Himsa.IMC2.DataDefinitions;
using System.Text;

namespace FakeIMC.Server
{
    internal static class LogHelper
    {
        internal static string Log(this MeasurementConditions conditions)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("MEASUREMENT CONDITIONS: ");

            var fInfo = conditions.GetType().GetFields();
            foreach (var info in fInfo)
            {
                var value = info.GetValue(conditions) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value);
            }

            return sb.ToString();
        }
    }
}
