using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using ORM.Pluming.Attributes;

namespace ORM.Pluming.Conventions
{
    public class VersionedConvention : IVersionConvention, IVersionConventionAcceptance
    {

        public void Apply(IVersionInstance instance)
        {
            instance.Column("Version");
            instance.UnsavedValue("0");
        }

        public void Accept(IAcceptanceCriteria<IVersionInspector> criteria)
        {
            criteria.Expect(a => a.EntityType.Has<IVersioned>());
        }
    }
}