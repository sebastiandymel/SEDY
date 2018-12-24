namespace ORM.Inheritence._01
{
    public class Product : Audited
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
    }
}