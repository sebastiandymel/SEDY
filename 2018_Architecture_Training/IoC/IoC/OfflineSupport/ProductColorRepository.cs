using System;
using System.Collections.Generic;

namespace IoC.OfflineSupport
{
    public class ProductColorRepository : IProductColorRepository
    {
        public bool _isDatabaseDown;

        public IList<ConsoleColor> GetColorsForUser()
        {
            if (_isDatabaseDown) return GetDetaultColors();
            return GetCusomizedColors();
        }

        private IList<ConsoleColor> GetCusomizedColors()
        {
            //bazując na użytkowniku dobieramy specjalne dla niego kolory
            return new List<ConsoleColor> {ConsoleColor.Cyan, ConsoleColor.DarkBlue};
        }

        private IList<ConsoleColor> GetDetaultColors()
        {
            return new List<ConsoleColor> {ConsoleColor.Black, ConsoleColor.White};
        }
    }
}