using ORM.Pluming;
using ORM.Pluming.Attributes;

namespace ORM.Versioning
{
    public class VersionedEntity : IEntity
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}