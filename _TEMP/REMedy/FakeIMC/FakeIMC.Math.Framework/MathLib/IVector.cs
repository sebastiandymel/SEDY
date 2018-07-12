using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FakeIMC.Math
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "We do not want IVector<T> to end in 'Collection' because that would sound like a collection of vectors")]
    public interface IVector<T> : IXmlSerializable, ICloneable, IEnumerable<T>
        where T : struct, IComparable<T>
    {
        T this[int index] { get; set; }
        T Last { get; set; }
        T First { get; set; }

        int Length { get; }
        bool Contains(T value);
        bool IsEmpty { get; }

        bool Equals(object value);
        int GetHashCode();

        T Min();
        T Max();

        void Flip();

        IVector<T> GetValues(int index, int count);
        void Initialize(T value);
    }
}
