using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace ORM.Pluming.Conventions
{
    /// <summary>
    ///   All properties marked with <see cref="RequiredAttribute" /> should be not null.
    /// </summary>
    public class NotNullPropertyConvention : AttributePropertyConvention<RequiredAttribute>
    {
        protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
        {
            var hasEntityParent = AppAutomappingCfg.IsEntity(instance.EntityType.BaseType);
            if (hasEntityParent == false)
            {
                instance.Not.Nullable();
            }
        }
    }
}