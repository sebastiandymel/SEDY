using System.Collections.Generic;

namespace SOLID.Shop.Model
{
    public class BookItem
    {
        public int Id { get; set; }
        public IList<string> Images { get; set; }
        public string Name { get; set; }
        public int Description { get; set; }

        public decimal Price { get; set; }

        //unique book identifier
        public string EAN { get; set; }
    }
}