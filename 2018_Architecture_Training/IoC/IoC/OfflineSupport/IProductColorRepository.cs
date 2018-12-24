using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.OfflineSupport
{
    public interface IProductColorRepository
    {
        IList<ConsoleColor> GetColorsForUser();
    }
}
