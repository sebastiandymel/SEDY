using ORM.Pluming;

namespace ORM.Inheritence._03
{
    public class Employee : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Surname { get; set; }
    }
}