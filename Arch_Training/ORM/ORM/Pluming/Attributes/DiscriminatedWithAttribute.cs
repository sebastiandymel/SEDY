using System;

namespace ORM.Pluming.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DiscriminatedWithAttribute : Attribute
    {
        private readonly string _value;
        public readonly string Value;

        public DiscriminatedWithAttribute(string value)
        {
            _value = value;
        }
    }
}