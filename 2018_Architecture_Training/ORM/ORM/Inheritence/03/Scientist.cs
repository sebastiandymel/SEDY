using System.ComponentModel.DataAnnotations;

namespace ORM.Inheritence._03
{
    public class Scientist : Employee
    {
        public virtual string Specialization { get; set; }
        public virtual string Division { get; set; }
        [Required] 
        public virtual string ClearanceLevel { get; set; }
    }
}