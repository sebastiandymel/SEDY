using System;

namespace FakeIMC.Math
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IVectorDoubleReadOnly : IVector<double>, ICloneable
    {
        void SetValuesAt(int[] indexes, IVectorDoubleReadOnly value);
        IVectorDoubleReadOnly GetValuesAt(params int[] indexes);

        void SetValuesAt(IVectorDoubleReadOnly indexes, IVectorDoubleReadOnly value);
        IVectorDoubleReadOnly GetValuesAt(IVectorDoubleReadOnly indexes);

        IVectorDoubleReadOnly Diff();
        IVectorDoubleReadOnly GreaterThan(IVectorDoubleReadOnly vector);
        IVectorDoubleReadOnly LesserThan(IVectorDoubleReadOnly vector);
        IVectorDoubleReadOnly LesserThan(double value);

        double Sum();
        double Sum(int index, int count);
        double Mean();
        double Mean(int index, int count);
        bool EqualsApproximately(IVectorDoubleReadOnly vector, double deviationFactor);
        bool EqualsExactly(IVectorDoubleReadOnly vector);

        IVectorDoubleReadOnly Find(double value, SearchPredicate predicate);
    }
}
