using System;
using ORM.Pluming;
using ORM.Pluming.Attributes;

namespace ORM.Inheritence._02
{
    public class Car : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string VIN { get; set; }
        public virtual string Manufacturer { get; set; }
        public virtual DateTime ConstructionDate { get; set; }
        public virtual decimal StickerPrice { get; set; }
    }
}