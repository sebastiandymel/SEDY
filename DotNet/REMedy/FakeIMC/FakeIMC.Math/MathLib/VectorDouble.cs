using System;
using System.Globalization;
using System.Text;

namespace FakeIMC.Math
{
    [Serializable] // Needed for debug visualizers
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class VectorDouble : Vector<double>, IVectorDoubleReadOnly
    {
        [Serializable]
        private class VectorHelpers : IVectorHelpers
        {
            #region IVectorHelpers Members

            /// <summary>
            /// Calculates an appropriate size of the internal array based on the needed size
            /// </summary>
            /// <param name="neededSize"></param>
            /// <returns>a size to use for the internal array</returns>
            public double[] GetDataArray(int neededSize)
            {
                // arrays of size 0 are very common. These shold be sized 0
                if (neededSize == 0)
                    return null;

                // Use a size from the sizings array. 
                for (int i = 0; i < VectorDouble.sizings.Length; i++)
                    if (neededSize <= VectorDouble.sizings[i])
                        return new double[VectorDouble.sizings[i]];

                // if the size is still not enough, set the size to what is needed.
                return new double[neededSize];
            }

            public void CopyFrom(double[] source, double[] target, int count)
            {
                if (target == null || source == null)
                    return;

                long longCount = (long)count * (long)VectorDouble.ELEMENTSIZE;

                if (longCount > int.MaxValue)
                    throw new ArgumentOutOfRangeException("count");

                Buffer.BlockCopy(source, 0, target, 0, (int)longCount);
            }

            #endregion
        }

        const int ELEMENTSIZE = sizeof(double);

        #region Fields & Properties

        public VectorDouble()
            : base(new VectorHelpers(), 0)
        { }

        public VectorDouble(int size)
            : base(new VectorHelpers(), size)
        { }

        public VectorDouble(int size, double value)
            : base(new VectorHelpers(), size)
        {
            Initialize(value);
        }

        public VectorDouble(params double[] vector)
            : base(new VectorHelpers(), vector)
        { }

        public VectorDouble(params int[] vector)
            : base(new VectorHelpers(), vector.Length)
        {
            for (int i = 0; i < vector.Length; i++) this.data[i] = vector[i];
        }

        public VectorDouble(IVectorDoubleReadOnly vector)
            : base(vector)
        { }

        // These steps are optimized for the best memory usage possible according to measurements done on Genie during a fitting session
        // The kneepoints are based on the most common sizes of Vectors. No Vector in genie has yet been measured to contain more 
        // than 10000 entries. And no Vector in Genie is resized larger than 256. Beyond 256, all Vectors are initialized with their final size.
        static readonly int[] sizings = new[] { 4, 10, 16, 24, 40, 95, 161, 200, 256 };

        public void Initialize(IVectorDoubleReadOnly vector)
        {
            base.Initialize((VectorDouble)vector);
        }

        /// <summary>
        /// Indexer used to get or set elements in a vector
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public IVectorDoubleReadOnly GetValuesAt(IVectorDoubleReadOnly indexes)
        {
            var vector = new VectorDouble(indexes.Length);
            for (int i = 0; i < indexes.Length; i++)
                vector[i] = this.data[(int)indexes[i]];

            return vector;
        }

        public void SetValuesAt(IVectorDoubleReadOnly indexes, IVectorDoubleReadOnly value)
        {
            for (int i = 0; i < indexes.Length; i++) this.data[(int)indexes[i]] = value[i];
        }

        /// <summary>
        /// Indexer used to get or set elements in a vector
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public IVectorDoubleReadOnly GetValuesAt(params int[] indexes)
        {
            var vector = new VectorDouble(indexes.Length);
            for (int i = 0; i < indexes.Length; i++)
                vector[i] = this.data[indexes[i]];

            return vector;
        }

        public void SetValuesAt(int[] indexes, IVectorDoubleReadOnly value)
        {
            for (int i = 0; i < indexes.Length; i++) this.data[indexes[i]] = value[i];
        }

        #endregion

        #region Public manipulation functions

        /// <summary>
        /// Used to add another vector to the vector
        /// </summary>
        /// <param name="vector">the vector to add</param>
        public void Add(IVectorDoubleReadOnly vector)
        {
            if (vector.Length != Length)
                throw new ArgumentException("Dimensions must agree.", "vector");

            for (int i = 0; i < Length; i++) this.data[i] += vector[i];
        }

        /// <summary>
        /// Used to add an array of values to the vector. The array of values must have same lenght as the vector
        /// </summary>
        /// <param name="values">the values to add</param>
        public void Add(params double[] values)
        {
            if (values.Length != Length)
                throw new ArgumentException("Dimensions must agree.", "values");

            for (int i = 0; i < Length; i++) this.data[i] += values[i];
        }

        /// <summary>
        /// Adds a value to all the values in the vector
        /// </summary>
        /// <param name="value">the value to add</param>
        public void Add(double value)
        {
            for (int i = 0; i < Length; i++) this.data[i] += value;
        }


        /// <summary>
        /// Returns a number of values, starting at startIndex
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override IVector<double> GetValues(int index, int count)
        {
            long indexLong = ((long)index * (long)VectorDouble.ELEMENTSIZE);
            long countLong = ((long)count * (long)VectorDouble.ELEMENTSIZE);

            if (indexLong > int.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            if (countLong > int.MaxValue)
                throw new ArgumentOutOfRangeException("count");

            var result = new VectorDouble(count);
            Buffer.BlockCopy(this.data, (int)indexLong, result.data, 0, (int)countLong);

            return result;
        }


        /// <summary>
        /// Used to Subtract another vector from the vector.
        /// </summary>
        /// <param name="vector">The vector to subtract.</param>
        public void Subtract(IVectorDoubleReadOnly vector)
        {
            if (vector.Length != Length)
                throw new ArgumentException("Dimensions must agree.", "vector");

            for (int i = 0; i < Length; i++) this.data[i] -= vector[i];
        }

        /// <summary>
        /// Subtract a value from all the values in the vector.
        /// </summary>
        /// <param name="value">The value to subtract.</param>
        public void Subtract(double value)
        {
            for (int i = 0; i < Length; i++) this.data[i] -= value;
        }

        /// <summary>
        /// Used to multiply another vector to the vector.
        /// </summary>
        /// <param name="vector">The vector to multiply by.</param>
        public void Multiply(IVectorDoubleReadOnly vector)
        {
            if (vector.Length == 1)
            {
                Multiply(vector[0]);
                return;
            }

            if (vector.Length != Length)
                throw new ArgumentException("The argument vector must have same length.", "vector");

            for (int i = 0; i < Length; i++) this.data[i] *= vector[i];
        }


        /// <summary>
        /// Multiply all the values in the vector by a value.
        /// </summary>
        /// <param name="factor">The factor to multiply by.</param>
        public void Multiply(double factor)
        {
            for (int i = 0; i < Length; i++) this.data[i] *= factor;
        }

        /// <summary>
        /// Divide each value in the vector with the value from another vector.
        /// </summary>
        /// <param name="vector">The vector to divide with.</param>
        public void Divide(IVectorDoubleReadOnly vector)
        {
            if (vector.Length != Length)
                throw new ArgumentException("Dimensions must agree", "vector");

            for (int i = 0; i < Length; i++) this.data[i] /= vector[i];
        }

        /// <summary>
        /// Divide all the values in the vector with a given factor.
        /// </summary>
        /// <param name="factor">The factor to divide by.</param>
        public void Divide(double factor)
        {
            for (int i = 0; i < Length; i++)
            {
                this.data[i] /= factor;
            }
        }

        public void Ceiling()
        {
            for (int i = 0; i < Length; i++)
                this.data[i] =System.Math.Ceiling(this.data[i]);
        }

        public void Floor()
        {
            for (int i = 0; i < Length; i++)
                this.data[i] =System.Math.Floor(this.data[i]);
        }

        /// <summary>
        /// Calculates the base 2 logarithm of each value of the vector.
        /// </summary>
        public void Log2()
        {
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] <= 0)
                    throw new InvalidOperationException("Vector values must be larger than zero.");

                this.data[i] =System.Math.Log(this.data[i], 2);
            }
        }

        /// <summary>
        /// Round each value of the vector.
        /// </summary>
        public void Round()
        {
            for (int i = 0; i < Length; i++)
                this.data[i] =System.Math.Round(this.data[i]);
        }

        public void Round(MidpointRounding mode)
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Round(this.data[i], mode);
        }

        public void Round(int decimals)
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Round(this.data[i], decimals);
        }

        public void Round(int decimals, MidpointRounding mode)
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Round(this.data[i], decimals, mode);
        }

        /// <summary>
        /// Square the data x^2 = x*x.
        /// </summary>
        public void Square()
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Pow(this.data[i], 2);
        }

        /// <summary>Squares all elements in the the data member.</summary>
        public void SquareRoot()
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Sqrt(this.data[i]);
        }

        public void Abs()
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Abs(this.data[i]);
        }

        public void Negate()
        {
            for (int i = 0; i < Length; i++) this.data[i] = -this.data[i];
        }

        #endregion

        #region Public non-mainpulation functions

        /// <summary>
        /// Compare the length with the dimensions of another vector.
        /// </summary>
        /// <param name="vector">VectorDouble to compare with</param>
        /// <returns>True if the dimensions are equal, otherwise false.</returns>
        public bool CompareDimensions(IVectorDoubleReadOnly vector)
        {
            return (Length == vector.Length);
        }

        /// <summary>
        /// Compares the values of the vector with the values of another.
        /// The if the values of the compared vector are deviationing more 
        /// than one deviation factor the vectors are considered to be different.
        /// Notice: that the default Equals implementation is not exact and may also be a little slower.
        /// </summary>
        /// <param name="obj">Object to be compared to.</param>
        /// <returns>True if the objects are equal, otherwhise false.</returns>
        public bool EqualsExactly(IVectorDoubleReadOnly vector)
        {
            // Compare reference
            if (ReferenceEquals(this, vector))
                return true;

            // Protect against null reference
            if (vector == null)
                return false;

            // Compare length
            if (!CompareDimensions(vector))
                return false;

            // Compare exact values
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] != vector[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compares the values of the vector with a scalar value.
        /// </summary>
        /// <param name="value">Value to compare to </param>
        /// <returns>True if each element is equal to value, otherwhise false.</returns>
        public bool EqualsExactly(double value)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] != value)
                    return false;
            }

            return true;
        }

        #endregion

        #region Public non-mainpulation functions
        /// <summary>
        /// Compares the values of the vector with the values of another.
        /// The if the values of the compared vector are deviationing more 
        /// than one deviation factor the vectors are considered to be different.
        /// </summary>
        /// <param name="vector">VectorDouble to compare with</param>
        /// <param name="deviationFactor">The accepted error</param>
        /// <returns>True if the objects are equal, otherwhise false</returns>
        public bool EqualsApproximately(IVectorDoubleReadOnly vector, double deviationFactor)
        {
            // Compare reference
            if (ReferenceEquals(this, vector))
                return true;

            // Protect against null reference
            if (vector == null)
                return false;

            // Compare length
            if (!CompareDimensions(vector))
                return false;

            // Compare values
            for (int i = 0; i < Length; i++)
            {
                double a = this.data[i];
                double b = vector[i];

                // Compare diviation 
                //(without invoking methodSystem.Math.Abs and with minimal risk of under- or overflow) 
                if (a > b)
                {
                    if (a > b + deviationFactor)
                        return false;
                }
                else
                    if (!(a >= b - deviationFactor)) // Notice negation is needed to handle comparison with NaN values
                        return false;             // by the IEEE 754 standard, where any comparison and 
            }                                     // "double.NaN == double.NaN" returns false!     

            return true;
        }

        /// <summary>
        /// Calculate the mean value of a subset of the vector.
        /// </summary>
        /// <param name="indexStart">The index of the start of the subset.</param>
        /// <param name="indexEnd">The index of the end of the subset.</param>
        /// <returns>The mean value.</returns>
        public double Mean(int index, int count)
        {
            double sum = 0;
            for (int i = 0; i < count; i++)
                sum += this.data[i + index];

            return sum / count;
        }

        /// <summary>
        /// Calculate the mean value of the vector.
        /// </summary>
        /// <returns>The mean value.</returns>
        public double Mean()
        {
            return Sum() / Length;
        }

        /// <summary>
        /// Calculate the standard deviation of the vector
        /// </summary>
        /// <returns>The standard deviation</returns>
        public double StandardDeviation()
        {
            if (Length == 0)
                return 0;

            double mean = Mean();

            ///////////////
            // Calculate the variance 
            double totalVariance = 0;
            for (int i = 0; i < Length; i++)
            {
                // For each value calculate the difference to the average value. 
                double dif = this.data[i] - mean;

                // Calculate the squares of these differences.
                double sqDif = dif * dif;

                totalVariance += sqDif;
            }

            // Find the average of the squared differences. 
            double variance = totalVariance / Length;

            return System.Math.Sqrt(variance);
        }

        /// <summary>
        /// Calculates the median value of a vector
        /// </summary>
        /// <returns></returns>
        public double Median()
        {
            VectorDouble sorted = Clone() as VectorDouble;
            sorted.Sort();

            int half = (int)System.Math.Floor(Length / 2.0);

            if (half * 2 == Length)
                return Mean(half, 2);
            else
                return sorted[half];
        }

        /// <summary>
        /// Returns the sum of the data.
        /// </summary>
        /// <returns>sum of values.</returns>
        public double Sum()
        {
            double sum = 0;

            for (int i = 0; i < Length; i++)
                sum += this.data[i];

            return sum;
        }

        public double Sum(int index, int count)
        {
            double sum = 0;
            for (int i = index; i < index + count; i++)
                sum += this.data[i];

            return sum;
        }

        public double PoweredSum()
        {
            return PoweredSum(0, Length - 1);
        }

        public double PoweredSum(int startIndex, int endIndex)
        {
            double sum = 0;
            for (int i = startIndex; i <= endIndex; i++)
                sum +=System.Math.Pow(10, this.data[i] / 10);

            return 10.0 *System.Math.Log10(sum);
        }
        #endregion

        #region Static Operators
        public static VectorDouble Ceiling(IVectorDoubleReadOnly vector)
        {
            var result = vector.Clone() as VectorDouble;
            result.Ceiling();
            return result;
        }

        public static VectorDouble Floor(IVectorDoubleReadOnly vector)
        {
            var result = vector.Clone() as VectorDouble;
            result.Floor();
            return result;
        }

        public static VectorDouble Round(IVectorDoubleReadOnly vector)
        {
            var result = vector.Clone() as VectorDouble;
            result.Round();
            return result;
        }

        public static VectorDouble Round(VectorDouble vector, MidpointRounding midpointRounding)
        {
            var result = vector.Clone() as VectorDouble;
            result.Round(midpointRounding);
            return result;
        }

        /// <summary>
        /// Used to Add two vectors
        /// </summary>
        /// <returns>A new vector with the sum of the to vectors</returns>
        public static VectorDouble Add(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Add(vector2);
            return result;
        }

        /// <summary>
        /// Used to Add a vector with a value
        /// </summary>
        /// <param name="vector1">the vector</param>
        /// <param name="value">the value to add</param>
        /// <returns>a new vector with the sum</returns>
        public static VectorDouble Add(IVectorDoubleReadOnly vector1, Double value)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Add(value);
            return result;
        }

        public static VectorDouble Max(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new ArgumentException("Dimensions must agree", "vector1");

            var result = new VectorDouble(vector1.Length);

            for (int index = 0; index < result.Length; index++)
                result[index] =System.Math.Max(vector1[index], vector2[index]);

            return result;
        }

        public static VectorDouble Min(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new InvalidOperationException("Dimensions must agree");

            var result = new VectorDouble(vector1.Length);

            for (int index = 0; index < result.Length; index++)
                result[index] =System.Math.Min(vector1[index], vector2[index]);

            return result;
        }

        /// <summary>
        /// Used to subtract two vectors
        /// </summary>
        /// <param name="vector1">the vector to subtract from</param>
        /// <param name="vector2">the vector to subtract with</param>
        /// <returns>A new vector  with the sum of the two vectors</returns>
        public static VectorDouble Subtract(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Subtract(vector2);
            return result;
        }

        /// <summary>
        /// Merges two vectors, omits repeated values and returns a new vector.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns>Returns new and sorted vector</returns>
        public static VectorDouble MergeVectors(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            if (vector1 == null && vector2 == null)
                throw new InvalidOperationException("Both vectors to merge cannot be null");

            if (vector1 == null || vector1.IsEmpty)
                return new VectorDouble(vector2);

            var result = new VectorDouble(vector1);
            result.Merge(vector2);
            return result;
        }

        /// <summary>
        /// Used to subtract a given value from a vector
        /// </summary>
        /// <param name="vector1">the vector to subtract from</param>
        /// <param name="value">the value to subtract from the vector</param>
        /// <returns>A new vector with the sum</returns>
        public static VectorDouble Subtract(IVectorDoubleReadOnly vector1, Double value)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Subtract(value);
            return result;
        }

        /// <summary>
        /// Used to multiply two vectors
        /// </summary>
        /// <param name="vector1">The vector to multiply to</param>
        /// <param name="vector2">The vector to multiply from</param>
        /// <returns>A new vector with the product of the two vectors</returns>
        public static VectorDouble Multiply(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Multiply(vector2);
            return result;
        }

        /// <summary>
        /// Used to multiply a vector with a given factor
        /// </summary>
        /// <param name="vector1">The vector to multiply to</param>
        /// <param name="factor">The factor to multiply with</param>
        /// <returns>A new vector with the product of the vector and the factor</returns>
        public static VectorDouble Multiply(IVectorDoubleReadOnly vector1, Double factor)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Multiply(factor);
            return result;
        }

        /// <summary>
        /// Used to divide one vector with another
        /// </summary>
        /// <param name="vector1">The vector to divide from</param>
        /// <param name="vector2">The vector to divide with</param>
        /// <returns>A new vector with the result of the division</returns>
        public static VectorDouble Divide(IVectorDoubleReadOnly vector1, IVectorDoubleReadOnly vector2)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Divide(vector2);
            return result;
        }

        /// <summary>
        /// Used to divide a vector with a given factor
        /// </summary>
        /// <param name="vector1">The vector to divide from</param>
        /// <param name="factor">The factor to divide with</param>
        /// <returns>A new vector with the result of the division</returns>
        public static VectorDouble Divide(IVectorDoubleReadOnly vector1, Double factor)
        {
            var result = vector1.Clone() as VectorDouble;
            result.Divide(factor);
            return result;
        }

        /// <summary>
        /// Copies vector within specified indexes
        /// </summary>
        /// <param name="inputVector">Input vector to be used for copying</param>
        /// <param name="startIndex">Vector index, where to start copying</param>
        /// <param name="endIndex">Vector index, where to end copying</param>
        /// <returns></returns>
        public static VectorDouble CopyVector(IVectorDoubleReadOnly inputVector, int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= inputVector.Length)
                throw new ArgumentException("Start index must be a index within the vector.", "startIndex");
            if (endIndex < startIndex || endIndex >= inputVector.Length)
                throw new ArgumentException("End index must be a index within the vector.", "endIndex");

            var newVector = new VectorDouble(endIndex - startIndex + 1);

            for (int i = startIndex; i <= endIndex; i++)
            {
                newVector[i - startIndex] = inputVector[i];
            }

            return newVector;
        }

        /// <summary>
        /// Diff(X), for a vector X, is [X(2)-X(1)  X(3)-X(2) ... X(n)-X(n-1)].
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "n", Justification = "n is correct")]
        public VectorDouble Diff(int nOrder)
        {
            var result = new VectorDouble(this);

            if (nOrder > 0)
            {
                for (int i = 0; i < nOrder; ++i)
                {
                    for (int j = 0; j < Length - 1 - i; j++)
                    {
                        result[j] = result[j + 1] - result[j];
                    }
                }
                result.Resize(Length - nOrder);
            }
            return result;
        }

        /// <summary>
        /// Diff(), for a VectorDouble, is [X(2)-X(1)  X(3)-X(2) ... X(n)-X(n-1)].
        /// </summary>
        /// <returns></returns>
        public IVectorDoubleReadOnly Diff()
        {
            if (Length == 0)
                return new VectorDouble();

            var result = new VectorDouble(Length - 1);
            for (int index = 0; index < result.Length; index++)
            {
                result[index] = this.data[index + 1] - this.data[index];
            }
            return result;
        }

        /// <summary>
        /// Find indices of nonzero elements
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static VectorDouble FindNonzeroIndexes(VectorDouble vector)
        {
            var result = new VectorDouble(vector.Length);
            int size = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i] != 0)
                {
                    result[size] = i;
                    size++;
                }
            }
            result.Resize(size);
            return result;
        }

        /// <summary>
        /// Does element by element comparisons between this and vector, and returns a new vector of the same size with elements set to logical 1 where the relation is true and elements set to logical 0 where it is not.  A and B must have the same dimensions unless one is a scalar.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public IVectorDoubleReadOnly GreaterThan(IVectorDoubleReadOnly vector)
        {
            if (Length != vector.Length)
                throw new ArgumentException("Vector dimensions must agree", "vector");

            var result = new VectorDouble(Length);

            for (int i = 0; i < Length; i++)
                result[i] = this.data[i] > vector[i] ? 1 : 0;

            return result;
        }

        /// <summary>
        /// This is a suggested function to replace FindIndices(GreaterThan(vector)) calls.
        /// 
        /// Please give your comments! TOJ
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public int[] IndexGreaterThan(IVectorDoubleReadOnly vector)
        {
            if (Length != vector.Length)
                throw new ArgumentException("Vector dimensions must agree", "vector");

            // Temporary construct an array of the vector length
            var result1 = new int[Length];
            int j = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] > vector[i])
                {
                    result1[j] = i;
                    j++;
                }
            }

            // No greater values found
            if (j == 0)
                return null;

            // Construct a result vector with the correct length
            var result2 = new int[j];
            for (int i = 0; i < j; i++)
                result2[i] = result1[i];

            return result2;
        }

        public VectorDouble GreaterThan(double value)
        {
            var result = new VectorDouble(Length);

            for (int i = 0; i < Length; i++)
                result[i] = this.data[i] > value ? 1 : 0;

            return result;
        }

        public IVectorDoubleReadOnly LesserThan(IVectorDoubleReadOnly vector)
        {
            if (Length != vector.Length)
                throw new ArgumentException("Vector dimensions must agree", "vector");

            var result = new VectorDouble(Length);

            for (int i = 0; i < Length; i++)
                result[i] = this.data[i] < vector[i] ? 1 : 0;

            return result;
        }

        public IVectorDoubleReadOnly LesserThan(double value)
        {
            var result = new VectorDouble(Length);

            for (int i = 0; i < Length; i++)
                result[i] = this.data[i] < value ? 1 : 0;

            return result;
        }

        /// <summary>
        /// Find the indices in the vector given by the value and predicate
        /// </summary>
        /// <returns>Returns a new vector with indices</returns>
        public IVectorDoubleReadOnly Find(double value, SearchPredicate predicate)
        {
            switch (predicate)
            {
                case SearchPredicate.EqualTo:
                    return FindEqualTo(value);

                case SearchPredicate.LesserThan:
                    return FindLesserThan(value);

                case SearchPredicate.GreaterThan:
                    return FindGreaterThan(value);

                case SearchPredicate.LesserThanOrEqual:
                    return FindLesserThanOrEqual(value);

                case SearchPredicate.GreaterThanOrEqual:
                    return FindGreaterThanOrEqual(value);

                default:
                    throw new InvalidOperationException("Not implemented predicate.");
            }
        }

        #region Private Find Index Methods

        private VectorDouble FindLesserThan(double value)
        {
            var result = new VectorDouble(Length);
            int matches = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] < value)
                {
                    result[matches] = i;
                    matches++;
                }
            }
            result.Resize(matches);
            return result;
        }

        private VectorDouble FindLesserThanOrEqual(double value)
        {
            var result = new VectorDouble(Length);
            int matches = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] <= value)
                {
                    result[matches] = i;
                    matches++;
                }
            }
            result.Resize(matches);
            return result;
        }

        private VectorDouble FindGreaterThanOrEqual(double value)
        {
            var result = new VectorDouble(Length);
            int matches = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] >= value)
                {
                    result[matches] = i;
                    matches++;
                }
            }
            result.Resize(matches);
            return result;
        }

        private VectorDouble FindGreaterThan(double value)
        {
            var result = new VectorDouble(Length);
            int matches = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] > value)
                {
                    result[matches] = i;
                    matches++;
                }
            }
            result.Resize(matches);
            return result;
        }

        private VectorDouble FindEqualTo(double value)
        {
            var result = new VectorDouble(Length);
            int matches = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] == value)
                {
                    result[matches] = i;
                    matches++;
                }
            }
            result.Resize(matches);
            return result;
        }

        #endregion

        /// <summary>
        /// Limits each value of the vector so that it is within the allowed min
        /// and max.
        /// </summary>
        /// <param name="min">The smallest allowed value.</param>
        /// <param name="max">The largest allowed value.</param>
        public void LimitMinMax(double min, double max)
        {
            if (max <= min)
                throw new InvalidOperationException("Minimum must be smaller then maximum.");

            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] < min) this.data[i] = min;

                if (this.data[i] > max) this.data[i] = max;
            }
        }

        /// <summary>
        /// Limits the vector so that its values do not exceed the values of the limitting vector
        /// </summary>
        /// <param name="limit">upper limit vector</param>
        public void UpperClip(IVectorDoubleReadOnly limit)
        {
            if (limit.Length != Length)
                throw new ArgumentException("The argument vector must have same length.");

            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] > limit[i]) this.data[i] = limit[i];
            }
        }

        /// <summary>
        /// Limits the vector so that its values do not get below the values of the limitting vector
        /// </summary>
        /// <param name="limit">lower limit vector</param>
        public void LowerClip(IVectorDoubleReadOnly limit)
        {
            if (limit.Length != Length)
                throw new ArgumentException("The argument vector must have same length.");

            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] < limit[i]) this.data[i] = limit[i];
            }
        }

        /// <summary>
        /// Limits the vector so that its values equals or exceeds the values of the limit value
        /// </summary>
        /// <param name="limit">lower limit value</param>
        public void LowerClip(double limit)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] < limit) this.data[i] = limit;
            }
        }

        /// <summary>
        /// Limits the vector so that its values do not exceed the values of the limit value
        /// </summary>
        /// <param name="limit">upper limit value</param>
        public void UpperClip(double limit)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i] > limit) this.data[i] = limit;
            }
        }

        public void Log10()
        {
            for (int i = 0; i < Length; i++) this.data[i] =System.Math.Log10(this.data[i]);
        }

        public static VectorDouble Abs(IVectorDoubleReadOnly vector)
        {
            var result = vector.Clone() as VectorDouble;
            result.Abs();
            return result;
        }

        /// <summary>
        /// Calculates the base 2 logarithm of each value of the vector.
        /// </summary>
        public static VectorDouble Log10(IVectorDoubleReadOnly vector)
        {
            var result = vector.Clone() as VectorDouble;
            result.Log10();
            return result;
        }

        public static VectorDouble Square(IVectorDoubleReadOnly vector)
        {
            var result = vector.Clone() as VectorDouble;
            result.Square();
            return result;
        }

        /// <summary>
        /// Returns a vector with the spcified numbers, raised by the vector
        /// </summary>
        public static VectorDouble Pow(double value, IVectorDoubleReadOnly vector)
        {
            var result = new VectorDouble(vector.Length);

            for (int i = 0; i < vector.Length; i++)
                result[i] =System.Math.Pow(value, vector[i]);

            return result;
        }

        public override object Clone()
        {
            return new VectorDouble(this);
        }

        public override bool IsEqual(IVector<double> other)
        {
            if (!IsLengthEqual(other))
                return false;

            for (int i = 0; i < Length; i++)
                if (!MathLib.IsEqualDoubles(other[i], this.data[i]))
                    return false;

            return true;
        }

        #endregion

        /// <summary>
        /// NB: Client must make sure that all copied indexes are within source and destination array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="count"></param>
        public override void Copy(double[] source, int sourceIndex, double[] destination, int destinationIndex, int count)
        {
            if (source == null || destination == null)
                return;

            long longIndexFrom = (long)sourceIndex * (long)VectorDouble.ELEMENTSIZE;
            long longIndexTo = (long)destinationIndex * (long)VectorDouble.ELEMENTSIZE;
            long longCount = (long)count * (long)VectorDouble.ELEMENTSIZE;

            if (longIndexFrom > int.MaxValue)
                throw new ArgumentOutOfRangeException("sourceIndex");
            if (longIndexTo > int.MaxValue)
                throw new ArgumentOutOfRangeException("destination");
            if (longCount > int.MaxValue)
                throw new ArgumentOutOfRangeException("destinationIndex");

            Buffer.BlockCopy(source, (int)longIndexFrom, destination, (int)longIndexTo, (int)longCount);
        }

        /// <summary>
        /// Returns a string representating the vector.
        /// </summary>
        /// <returns>The string showing data in the vector.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < Length; i++)
            {
                if (i != 0)
                    stringBuilder.Append(", ");
                stringBuilder.Append(this.data[i].ToString(CultureInfo.InvariantCulture));
            }
            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
