using System;
using ORM.Pluming.Attributes;

namespace ORM.Inheritence._02
{
    public class NewCar : Car
    {
        public virtual DateTime AvailableFrom { get; set; }
        public virtual bool WasRefreshed { get; set; }
    }
}