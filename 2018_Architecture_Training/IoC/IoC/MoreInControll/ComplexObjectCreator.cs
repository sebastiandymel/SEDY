using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.MoreInControll
{
    public class ComplexObjectCreator
    {
        public ComplexObjectThatHasToBeCreatedManualy Create()
        {
            return new ComplexObjectThatHasToBeCreatedManualy();
        }
    }
}
