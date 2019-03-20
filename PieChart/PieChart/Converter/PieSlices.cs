using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PieChart
{
    public class PieSlices : IEnumerable<IPieSlice>
    {
        private List<IPieSlice> slices;

        public PieSlices()
        {
        }

        public PieSlices(string input)
        {
            var splitted = input.Split(',');
            this.slices= splitted.Select(x => new PieSlice { Value = double.Parse(x) }).ToList<IPieSlice>();
        }

        public IEnumerator<IPieSlice> GetEnumerator()
        {
            return this.slices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.slices.GetEnumerator();
        }
    }
}
