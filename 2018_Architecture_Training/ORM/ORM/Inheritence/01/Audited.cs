using System;
using ORM.Pluming;

namespace ORM.Inheritence._01
{
    public abstract class Audited : IEntity
    {
        public virtual int Id { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string CreateUser { get; set; }
        public virtual string UpdateUser { get; set; }
    }
}