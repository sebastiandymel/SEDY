using ORM.Pluming.Attributes;

namespace ORM.Inheritence._02
{
    [DiscriminatedWith("UsedCar")]
    public class UsedCar : Car
    {
        public virtual bool IsFirstOwner { get; set; }
        public virtual string LastOwnerName { get; set; }
        public virtual string LastOwnerSurname { get; set; }
    }
}