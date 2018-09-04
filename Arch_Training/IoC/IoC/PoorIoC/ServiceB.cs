using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;

namespace IoC.PoorIoC
{
    public class ServiceB
    {
        public ServiceB(IServiceA serviceA)
        {
        }
    }
}
