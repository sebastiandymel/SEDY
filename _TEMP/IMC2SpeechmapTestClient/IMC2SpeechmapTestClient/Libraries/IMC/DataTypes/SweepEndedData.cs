using System.Collections.Generic;
using Himsa.IMC2.DataDefinitions;

namespace IMC2SpeechmapTestClient.Libraries.IMC.DataTypes
{
    public class SweepEndedData
    {
        public Sideable<int?> AidedSii { get; set; }

        public Sideable<int?> UnaidedSii { get; set; }

        public IEnumerable<MeasurementPoint> Data { get; set; }

        public MeasurementSide Side { get; set; }
    }
}
