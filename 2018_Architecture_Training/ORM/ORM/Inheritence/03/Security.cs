namespace ORM.Inheritence._03
{
    public class Security : Employee
    {
        public virtual bool IsArmed { get; set; }
        public virtual string SecurityAgencyName { get; set; }
    }
}