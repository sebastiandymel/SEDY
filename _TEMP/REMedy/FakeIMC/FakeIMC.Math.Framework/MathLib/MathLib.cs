using System;

namespace FakeIMC.Math
{
    public static partial class MathLib
    {
        #region Constants

        private const double VERY_SMALL_NUMBER = 0.00000000001;
        public const double Epsilon = 0.00000000000000022204460492503131000;
        public const double Log10Of2 = 0.30102999566398119521373889472449;
        public const double Log2ToDb = 10.0 * MathLib.Log10Of2;

        #endregion

        # region General mathematical functions

        public static double DecibelToLinear(double value)
        {
            return System.Math.Pow(10.0, value * 0.05);
        }

        public static double LinearToDecibel(double value)
        {
            if (value < 0.0)
                throw new ArgumentException("Mathlib.LinearToDB: value < 0.0", "value");

            return 20.0 *System.Math.Log10(value);
        }

        public static double PowerLinearToDecibel(double value)
        {
            if (value < 0.0)
                throw new ArgumentException("Mathlib.PowerLinearToDB: value < 0.0", "value");

            return 10.0 *System.Math.Log10(value);
        }

        public static bool IsEqualDoubles(double value1, double value2)
        {
            return (System.Math.Abs(value1 - value2) < MathLib.VERY_SMALL_NUMBER);
        }

        /// <summary>
        /// Returns the mean value of the supplied values.
        /// </summary>
        /// <param name="values">At least one value.</param>
        /// <returns>The largest value.</returns>
        public static double Mean(params double[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("It is not legal to supply zero length input.", "values");

            double sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }

        /// <summary>
        /// Returns the largest value of the supplied values.
        /// </summary>
        /// <param name="values">At least one value.</param>
        /// <returns>The largest value.</returns>
        public static double Max(params double[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("It is not legal to supply zero length input.", "values");

            double maximumValue = Double.MinValue;
            for (int i = 0; i < values.Length; i++)
                maximumValue =System.Math.Max(values[i], maximumValue);

            return maximumValue;
        }

        /// <summary>
        /// Returns the smallest value of the supplied values.
        /// </summary>
        /// <param name="values">At least one value.</param>
        /// <returns>The smallest value.</returns>
        public static double Min(params double[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("It is not legal to supply zero length input.", "values");

            double minimumValue = Double.MaxValue;
            for (int i = 0; i < values.Length; i++)
                minimumValue =System.Math.Min(values[i], minimumValue);

            return minimumValue;
        }

        public static double PoweredSum(params double[] values)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum +=System.Math.Pow(10, values[i] / 10);

            return 10 *System.Math.Log10(sum);
        }

        /// <summary>
        /// Calculates average vector for gieven vectors.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>Average of coefficients.</returns>
        public static IVectorDoubleReadOnly Mean(params IVectorDoubleReadOnly[] vectors)
        {
            VectorDouble result = new VectorDouble(vectors[0].Length);

            for (int i = 0; i < vectors.Length; i++)
                result.Add(vectors[i]);

            result.Divide(vectors.Length);

            return result;
        }

        /// <summary>
        /// Calculates the median vector for the given vectors.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns>median vector</returns>
        public static IVectorDoubleReadOnly Median(params IVectorDoubleReadOnly[] vectors)
        {
            VectorDouble result = new VectorDouble(vectors[0].Length);

            // For each index find the median of all the vectors.
            for (int i = 0; i < result.Length; i++)
            {
                int numberOfVectors = vectors.Length;
                
                double[] x = new double[numberOfVectors];

                // Make an array x with the values from all the vectors for a given index i
                for (int j = 0; j < numberOfVectors; j++)
                    x[j] = vectors[j][i];

                Array.Sort(x); // x is a sorted array of index i of the vectors i.e. the "column" with index i.

                // If the number of values in the sorted array is odd the median value is the one with the middle index,
                // otherwise it is the average of the values with the two middle indeces.
                int mid = numberOfVectors / 2;
                result[i] = (numberOfVectors % 2 != 0) ? x[mid] : (x[mid - 1] + x[mid]) / 2;
            }

            return result;
        }

        /// <summary>
        /// Sums an array of integers
        /// </summary>
        /// <param name="numbers">The numbers to sum</param>
        /// <returns>Returns the sum of the numbers</returns>
        public static int Sum(params int[] numbers)
        {
            int sum = 0;
            for (int i = 0; i < numbers.Length; i++)
                sum += numbers[i];

            return sum;
        }

        public static int Pow2(int power)
        {
            if (power < 0 || power > 31)
                throw new ArgumentException("The power needs to be equal or larger than zero and at most 31 bit.", "power");

            return 1 << power;
        }

        #endregion

        #region Matlab functions
        /// <summary>
        /// Matlab function. CUMSUM(X) is a vector containing the cumulative sum of
        /// the elements of X.
        /// </summary>
        /// <param name="vector">The vector to build the cumsum upon.</param>
        /// <returns>A vector containing the cumsum.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cumsum", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static VectorDouble Cumsum(IVectorDoubleReadOnly vector)
        {
            VectorDouble tempVector = new VectorDouble(vector.Length);
            double sum = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                sum += vector[i];
                tempVector[i] = sum;
            }
            return tempVector;
        }


        /// <summary>
        /// Matlab function. GAMMA(X) evaluates the gamma function on the
        /// supplied value. The gamma function is defined as:
        ///     gamma(x) = integral from 0 to inf of t^(x-1) exp(-t) dt.
        /// 
        /// Implementation by Takuya OOURA (http://momonga.t.u-tokyo.ac.jp/~ooura/)
        /// </summary>
        /// <param name="x">The value to evaluate the gamma function on.</param>
        /// <returns>The gamma value of x.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "x", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static double Gamma(double x)
        {
            int n = (int)(x < 1.5 ? -2.5 + x : x - 1.5);

            double w = x - (n + 2);

            double y = ((((((((((((-1.99542863674e-7 * w + 1.337767384067e-6) * w -
                2.591225267689e-6) * w - 1.7545539395205e-5) * w +
                1.45596568617526e-4) * w - 3.60837876648255e-4) * w -
                8.04329819255744e-4) * w + 0.008023273027855346) * w -
                0.017645244547851414) * w - 0.024552490005641278) * w +
                0.19109110138763841) * w - 0.233093736421782878) * w -
                0.422784335098466784) * w + 0.99999999999999999;

            if (n > 0)
            {
                w = x - 1;
                for (int k = 2; k <= n; k++)
                    w *= x - k;
            }
            else
            {
                w = 1;
                for (int k = 0; k > n; k--)
                    y *= x - k;
            }

            return w / y;
        }

        /// <summary>
        /// Matlab function. GAMMA(X) evaluates the gamma function on each
        /// element of X. The gamma function is defined as:
        ///     gamma(x) = integral from 0 to inf of t^(x-1) exp(-t) dt.
        /// 
        /// Implementation by Takuya OOURA (http://momonga.t.u-tokyo.ac.jp/~ooura/)
        /// </summary>
        /// <param name="vector">A vector with elements to evaluate the gamma function on.</param>
        /// <returns>A vector with gamma values of supplied vector.</returns>
        public static VectorDouble Gamma(IVectorDoubleReadOnly vector)
        {
            VectorDouble tempVector = new VectorDouble(vector.Length);

            for (int i = 0; i < tempVector.Length; i++)
                tempVector[i] = Gamma(vector[i]);

            return tempVector;
        }
        /// <summary>
        /// Matlab function. rayleigh_square_density(x, n, sigma2)
        /// p = rayleigh_density(x, n)
        /// p = rayleigh_density(x, n, sigma2)
        /// rayleigh density function
        /// The rayleigh distribution is the distribution of the square root of a chi square distributed variable.
        /// p(x) = 2/( gamma(k) * beta^k ) * x^(2k-1) * exp( -x^2/beta )
        /// </summary>
        public static VectorDouble RayleighDensity(VectorDouble binCenters, double chiSquareOrder, double variance)
        {
            VectorDouble squareDensity = new VectorDouble(binCenters.Length);
            double k = chiSquareOrder / 2;
            double doubleK = 2 * k;
            double beta = 2 * variance;
            double kGamma = Gamma(k);
            double betaK =System.Math.Pow(beta, k);
            double gammmaBetaK = kGamma * betaK;
            double dx = binCenters[1] - binCenters[0];

            for (int i = 0; i < binCenters.Length; i++)
            {
                if (binCenters[i] > 0)
                    squareDensity[i] = 2 / gammmaBetaK *System.Math.Pow(binCenters[i], doubleK - 1) *System.Math.Exp(-System.Math.Pow(binCenters[i], 2) / beta);
                else if (binCenters[i] == 0)
                {
                    if (binCenters.Length > 1)
                        squareDensity[i] = 1 / gammmaBetaK * (System.Math.Pow(dx / 2, doubleK) / dx / k);
                    else
                        squareDensity[i] = 0;
                }
            }
            return squareDensity;
        }

        /// <summary>
        /// Overloaded, see above.
        /// </summary>
        public static VectorDouble RayleighDensity(VectorDouble binCenters, int chiSquareOrder)
        {
            return RayleighDensity(binCenters, chiSquareOrder, 1);
        }

        /// <summary>
        /// Matlab function. chi_square_density(x, n, sigma2)
        /// p = chi_square_density(x, n)
        /// p = chi_square_density(x, n, sigma2)
        /// chi square density function
        /// Inputs: 
        ///  x            Bin centers
        ///  n            Chi square order
        ///  sigma2       Variance of Gaussians that were added to get the Chi square
        /// Output:
        ///  p            Chi square density ( except in bin with center 0 where the density is replaced by 
        ///               integral over bin divided by bin width).
        /// </summary>
        public static VectorDouble ChiSquareDensity(VectorDouble binCenters, double chiSquareOrder, double variance)
        {
            VectorDouble squareDensity = new VectorDouble(binCenters.Length);
            double k = chiSquareOrder / 2;
            double beta = 2 * variance;
            double kGamma = Gamma(k);
            double betaK =System.Math.Pow(beta, k);
            double gammmaBetaK = kGamma * betaK;
            double dx = binCenters[1] - binCenters[0];

            for (int i = 0; i < binCenters.Length; i++)
            {
                if (binCenters[i] > 0)
                    squareDensity[i] = 1 / gammmaBetaK *System.Math.Pow(binCenters[i], k - 1) *System.Math.Exp(-binCenters[i] / beta);
                else if (binCenters[i] == 0)
                {
                    if (binCenters.Length > 1)
                        squareDensity[i] = 1 / gammmaBetaK * (System.Math.Pow(dx / 2, k) / dx / k);
                    else
                        squareDensity[i] = 0;
                }
            }
            return squareDensity;
        }

        /// <summary>
        /// Overloaded, see above.
        /// </summary>
        public static VectorDouble ChiSquareDensity(VectorDouble binCenters, int chiSquareOrder)
        {
            return ChiSquareDensity(binCenters, chiSquareOrder, 1);
        }

        /// <summary>
        /// Matlab function. LINSPACE(X1, X2) generates a row vector of 100 linearly
        /// equally spaced points between X1 and X2.
        /// </summary>
        /// <param name="fromValue">The value to start from.</param>
        /// <param name="toValue">The value to ned with.</param>
        /// <returns>A vector containing 100 elements between fromValue and toValue.</returns>
        public static VectorDouble Linspace(double fromValue, double toValue)
        {
            return Linspace(fromValue, toValue, 100);
        }

        /// <summary>
        /// Matlab function. LINSPACE(X1, X2, N) generates N points between X1 and X2. 
        /// For N &lt; 2, LINSPACE returns X2.
        /// </summary>
        /// <param name="fromValue">The value to start from.</param>
        /// <param name="toValue">The value to ned with.</param>
        /// <param name="numberOfValues">The number of values in the return vector.</param>
        /// <returns>A vector containing the specified number of elements
        /// between fromValue and toValue.</returns>
        public static VectorDouble Linspace(double fromValue, double toValue, int numberOfValues)
        {
            if (numberOfValues < 2)
                return new VectorDouble(new double[] { toValue });

            VectorDouble tempVector = new VectorDouble(numberOfValues);
            double gap = (toValue - fromValue) / (numberOfValues - 1);

            for (int i = 0; i < numberOfValues; i++)
            {
                tempVector[i] = fromValue + gap * i;
            }

            return tempVector;
        }

        /// <summary>
        /// Matlab function. LINSPACE2(X1, N, X2) generates N points between X1 and X2. 
        /// For N &lt; 2, LINSPACE returns X2. Don't use for large 
        /// </summary>
        /// <param name="fromValue">The value to start from.</param>
        /// <param name="toValue">The value to end with.</param>
        /// <param name="valueSpacing">The distance between values.</param>
        /// <returns>A vector containing the specified number of elements
        /// between fromValue and toValue.</returns>
        public static VectorDouble Linspace2(double fromValue, double valueSpacing, double toValue)
        {
            if (valueSpacing == 0.0)
                return new VectorDouble();

            if (toValue == fromValue)
                return new VectorDouble(new double[] { toValue });

            int numberOfValues = (int)System.Math.Floor((toValue - fromValue) / valueSpacing) + 1;
            if (numberOfValues < 2)
                return new VectorDouble();

            VectorDouble tempVector = new VectorDouble(numberOfValues);
            tempVector[0] = fromValue;

            for (int i = 1; i < numberOfValues; i++)
            {
                tempVector[i] = tempVector[i - 1] + valueSpacing;
            }

            return tempVector;
        }

        /// <summary>
        /// Matlab function. Y = POLYVAL(P,X) returns a vector with value of a
        /// polynomial P evaluated at each X value. P is a vector of length N+1
        /// whose elements are the coefficients of the polynomial in descending
        /// powers.
        /// </summary>
        /// <param name="factors"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "Polyval", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static VectorDouble Polyval(IVectorDoubleReadOnly factors, IVectorDoubleReadOnly variables)
        {
            VectorDouble tempVector = new VectorDouble(variables.Length);

            for (int i = 0; i < variables.Length; i++)
                tempVector[i] = Polyval(factors, variables[i]);

            return tempVector;
        }

        /// <summary>
        /// Matlab function. Y = POLYVAL(P,X) returns the value of a polynomial
        /// P evaluated at X. P is a vector of length N+1 whose elements are the
        /// coefficients of the polynomial in descending powers.
        /// 
        /// The calculation is reversed in order to avoid costly Pow operations
        /// on the variable. This makes the function hard to read, but helps on
        /// performance. (TOJ)
        /// </summary>
        /// <param name="factors"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "Polyval", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static double Polyval(IVectorDoubleReadOnly factors, double variable)
        {
            double polynomial = factors[factors.Length - 1];
            double powerValue = variable;

            for (int i = (factors.Length - 2); i >= 0; i--)
            {
                polynomial += powerValue * factors[i];
                powerValue *= variable;
            }
            return polynomial;
        }



        /// <summary>
        ///Y = FILTER(B,A,X) filters the data in vector X with the
        ///filter described by vectors A and B to create the filtered
        ///data Y.  The filter is a "Direct Form II Transposed"
        ///implementation of the standard difference equation:

        ///a(1)*y(n) = b(1)*x(n) + b(2)*x(n-1) + ... + b(nb+1)*x(n-nb)
        ///						- a(2)*y(n-1) - ... - a(na+1)*y(n-na)
        ///
        ///If a(1) is not equal to 1, FILTER normalizes the filter
        ///coefficients by a(1). 
        ///
        ///FILTER always operates along the first non-singleton dimension,
        ///namely dimension 1 for column vectors and non-trivial matrices,
        ///and dimension 2 for row vectors.
        ///
        ///[Y,Zf] = FILTER(B,A,X,Zi) gives access to initial and final
        ///conditions, Zi and Zf, of the delays.  Zi is a vector of length
        ///MAX(LENGTH(A),LENGTH(B))-1, or an array with the leading dimension 
        ///of size MAX(LENGTH(A),LENGTH(B))-1 and with remaining dimensions 
        ///matching those of X.
        ///
        ///FILTER(B,A,X,[],DIM) or FILTER(B,A,X,Zi,DIM) operates along the
        ///dimension DIM.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Scope = "member", Target = "FakeIMC.Math.MathLib.#Filter(FakeIMC.Math.IVectorDoubleReadOnly,FakeIMC.Math.IVectorDoubleReadOnly,FakeIMC.Math.IVectorDoubleReadOnly)", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static VectorDouble Filter(IVectorDoubleReadOnly b, IVectorDoubleReadOnly a, IVectorDoubleReadOnly x)
        {
            return Filter(new VectorDouble(x.Length), b, a, x);
        }

        /// <summary>
        /// See other function.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Scope = "member", Target = "FakeIMC.Math.MathLib.#Filter(FakeIMC.Math.VectorDouble,FakeIMC.Math.IVectorDoubleReadOnly,FakeIMC.Math.IVectorDoubleReadOnly,FakeIMC.Math.IVectorDoubleReadOnly)", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static VectorDouble Filter(VectorDouble y, IVectorDoubleReadOnly b, IVectorDoubleReadOnly a, IVectorDoubleReadOnly x)
        {
            if (b.Length == 0)
                throw new ArgumentException("The filter implementation does not support none B-coeffients.", "b");
            if (a.Length == 0)
                throw new ArgumentException("The filter implementation does not support none A-coeffients.", "a");
            if (a[0] == 0)
                throw new ArgumentException("The filter implementation in matlib does not support 0.0 in the first value of A-coeffients.", "a");
            if (ReferenceEquals(x, y))
                throw new InvalidOperationException("Input vector X and output vector Y refer to the same address.");

            // Resize Y if the provided vector is too small.
            if (y.Length < x.Length)
                y.Resize(x.Length);

            int nx = x.Length;
            int nb = b.Length;
            int na = a.Length;

            if (nx > 0)
            {
                IVectorDoubleReadOnly bNorm = b;
                IVectorDoubleReadOnly aNorm = a;

                // Normalize
                if (a[0] != 1.0)
                {
                    VectorDouble aTemp = a.Clone() as VectorDouble;
                    VectorDouble bTemp = b.Clone() as VectorDouble;

                    aTemp.Divide(a[0]); // Note, division could be performed inplace in the following 
                    bTemp.Divide(a[0]); // filter calculation. Doing so would save us from a cloning, 
                    // but make the code a little less pure.                                       
                    aNorm = aTemp;
                    bNorm = bTemp;
                }

                for (int n = 0; n < nx; n++)
                {
                    double result = bNorm[0] * x[n];
                    for (int j = 1; j <= n; j++)
                    {
                        result += (j < nb ? bNorm[j] : 0) * x[n - j];
                        result -= (j < na ? aNorm[j] : 0) * y[n - j];
                    }
                    y[n] = result;
                }
            }
            return y;
        }
        /// <summary>
        ///   CONV Convolution and polynomial multiplication.
        ///   C = CONV(A, B) convolves vectors A and B.  The resulting
        ///   vector is length LENGTH(A)+LENGTH(B)-1.
        ///   If A and B are vectors of polynomial coefficients, convolving
        ///   them is equivalent to multiplying the two polynomials.
        ///
        /// /// </summary>
        /// <param name="a">Vector A.</param>
        /// <param name="b">Vector B.</param>
        /// <returns>Convoled output.</returns> 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "a", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "b", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static VectorDouble Convolute(IVectorDoubleReadOnly a, IVectorDoubleReadOnly b)
        {
            // Convolution, polynomial multiplication, and FIR digital
            // filtering are all the same operations.  Since FILTER
            // is a fast built-in primitive, we'll use it for CONV.
            if (b.Length == 0)
                throw new ArgumentException("The filter implementation does not support none B-coeffients.", "b");
            if (a.Length == 0)
                throw new ArgumentException("The filter implementation does not support none A-coeffients.", "a");

            int maxK = b.Length + a.Length - 1;
            VectorDouble result = new VectorDouble(maxK);

            for (int k = 0; k < maxK; k++)
            {
                double t = 0;
                int startIndex =System.Math.Max(0, k + 1 - b.Length);
                for (int j = startIndex; (j <= k) && (j < a.Length); j++)
                    t += a[j] * b[k - j];

                result[k] = t;
            }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "a", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "b", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static VectorDouble Convolute(IVectorDoubleReadOnly a, double b)
        {
            VectorDouble bVector = new VectorDouble(1) {[0] = b};
            return Convolute(a, bVector);
        }

        #endregion

        #region Interpolation

        public static VectorDouble Interpolate(IVectorDoubleReadOnly underlyingXValues, IVectorDoubleReadOnly underlyingYValues, IVectorDoubleReadOnly interpolateAtXValues, InterpolateType type)
        {
            if (underlyingXValues.Length != underlyingYValues.Length)
                throw new InvalidOperationException("Dimensions of the two vectors mush agree.");

            VectorDouble destination = new VectorDouble(interpolateAtXValues.Length);
            for (int i = 0; i < interpolateAtXValues.Length; i++)
                destination[i] = Interpolate(underlyingXValues, underlyingYValues, interpolateAtXValues[i], type);

            return destination;
        }

        public static double LogInterpolate(double previousXValue, double previousYValue, double nextXValue, double nextYValue, double interpolateAtXValue)
        {
            double alfa = 0.0;

            if (previousXValue == nextXValue || // Extrapolation,  
                previousYValue == nextYValue)   // Flat curve (horizontal line)
            {
                return previousYValue;
            }
            else
            {
                double log10_freq_next_div_prev =System.Math.Log10(nextXValue / previousXValue);

                // logarithmic interpolation factors
                alfa =System.Math.Log10(interpolateAtXValue / previousXValue) / log10_freq_next_div_prev;
            }

            // logarithmic interpolation
            return previousYValue * (1.0-alfa) + nextYValue * alfa;
        }

        public static double LinearInterpolate(double previousXValue, double previousYValue, double nextXValue, double nextYValue, double interpolateAtXValue)
        {
            double val = previousYValue;

            // If not flat curve or extrapolation
            if (previousXValue != nextXValue && previousYValue != nextYValue)
            {
                //        y2 - y1
                // alfa = -------
                //        x2 - x1
                double alfa = (nextYValue - previousYValue) / (nextXValue - previousXValue);

                // y(x) = alfa * x + c
                val = (interpolateAtXValue - previousXValue) * alfa + previousYValue;
            }

            return val;
        }

        /// <summary>
        /// Implementation of Matlas function 'LOGINTERP'. Equivalent to the 'LOGINTERP(X, Y, XI, 2, 2)'.
        /// LogExtrapolation based on first or last two points.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xi"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "x", Justification = "The name 'x' is intended and a better name does not exist.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "y", Justification = "The name 'y' is intended and a better name does not exist.")]
        public static VectorDouble LogInterpolation(IVectorDoubleReadOnly x, IVectorDoubleReadOnly y, IVectorDoubleReadOnly xi)
        {
            // Code based on /Matlas/All/Common/LogInterp.m
            VectorDouble yi;

            //% Only one point: Flat extrapolation
            if (x.Length == 1)
            {
                yi = new VectorDouble(1, 1);
            }
            else if (x.Length == 0)
            {
                yi = new VectorDouble();
            }
            else
            {
                yi = new VectorDouble(xi.Length);
            }
            // % LF slope
            double LFSlope = (y[1] - y[0]) /System.Math.Log(x[1] / x[0]); // % Calculate slope (natural log)
            // % HF slope
            double HFSlope = (y[y.Length - 1] - y[y.Length - 2]) /System.Math.Log(x[x.Length - 1] / x[x.Length - 2]);

            int j = 0;

            for (int i = 0; i < xi.Length; i++)
            {
                if (xi[i] <= x[0])
                    yi[i] = y[0] + LFSlope *System.Math.Log(xi[i] / x[0]);
                else if (xi[i] >= x[x.Length - 1])
                    yi[i] = y[y.Length - 1] + HFSlope *System.Math.Log(xi[i] / x[x.Length - 1]);
                else
                {
                    if (i > 0 && xi[i] < xi[i - 1])
                        while (x[j] > xi[i])
                            j--;

                    for (; j < x.Length; j++)
                    {
                        if (x[j] > xi[i])
                            break;
                    }
                    //  %Change here not neeed for log10 since log10 = log / log (10) (2 divisions less) see log10.m
                    yi[i] =
                        y[j] + (y[j - 1] - y[j]) *System.Math.Log(xi[i] / x[j]) /
                       System.Math.Log(x[j - 1] / x[j]);
                }
            }

            return yi;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "x", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "y", Justification = "This naming is intended and makes sense in the Mathematical context.")]
        public static double Interpolate(IVectorDoubleReadOnly xValues, IVectorDoubleReadOnly yValues, double interpolateAtXValue, InterpolateType type)
        {
            if (xValues.Length != yValues.Length)
                throw new InvalidOperationException("Dimensions of the two vectors mush agree.");

            return RecursiveInterpolate(xValues, yValues, 0, xValues.Length - 1, interpolateAtXValue, type);
        }

        private static double RecursiveInterpolate(IVectorDoubleReadOnly xValues, IVectorDoubleReadOnly yValues, int startX, int endX, double interpolateAtXValue, InterpolateType type)
        {
            // If "on bounds" or "out of bounds", interpolation is not necessary. Just use Y value from nearest boundary
            if (interpolateAtXValue <= xValues[startX])
                return yValues[startX];

            else if (interpolateAtXValue >= xValues[endX])
                return yValues[endX];

            // If the x value is between the first two values, do the interpolation
            else if (interpolateAtXValue > xValues[startX] && interpolateAtXValue < xValues[startX + 1])
            {
                switch (type)
                {
                    case InterpolateType.Logarithmic:
                        return LogInterpolate(xValues[startX], yValues[startX], xValues[startX + 1], yValues[startX + 1], interpolateAtXValue);
                    case InterpolateType.Linear:
                        return LinearInterpolate(xValues[startX], yValues[startX], xValues[startX + 1], yValues[startX + 1], interpolateAtXValue);
                    default:
                        throw new NotImplementedException();
                }
            }

            // else, DIVIDE AND CONQUER
            int midIndex = (endX + startX) >> 1;

            if (interpolateAtXValue > xValues[midIndex])
                return RecursiveInterpolate(xValues, yValues, midIndex, endX, interpolateAtXValue, type);

            else if (interpolateAtXValue < xValues[midIndex])
                return RecursiveInterpolate(xValues, yValues, startX + 1, midIndex, interpolateAtXValue, type); /* We already tried startX<>startX+1 */

            else return yValues[midIndex];
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "x",
            Justification = "This naming is intended and makes sense in the Mathematical context.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "y",
            Justification = "This naming is intended and makes sense in the Mathematical context.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
            "CA2204:Literals should be spelled correctly", MessageId = "xValues",
            Justification = "This is the parameter name.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
            "CA2204:Literals should be spelled correctly", MessageId = "yValues",
            Justification = "This is the parameter name.")]
        public static VectorDouble LogInterpolate(IVectorDoubleReadOnly xValues, IVectorDoubleReadOnly yValues,
            IVectorDoubleReadOnly xiValues)
        {
            if (xValues.Length != yValues.Length)
                throw new InvalidOperationException("Vectors xValues and yValues must be of the same length.");

            VectorDouble yi = new VectorDouble(xiValues.Length);

            int j = 0;

            for (int i = 0; i < xiValues.Length; i++)
            {
                if (xiValues[i] <= xValues[0])
                    yi[i] = yValues[0];
                else if (xiValues[i] >= xValues[xValues.Length - 1])
                    yi[i] = yValues[yValues.Length - 1];
                else
                {
                    if (i > 0 && xiValues[i] < xiValues[i - 1])
                        while (xValues[j] > xiValues[i])
                            j--;

                    for (; j < xValues.Length; j++)
                    {
                        if (xValues[j] > xiValues[i])
                            break;
                    }

                    yi[i] =
                        yValues[j] + (yValues[j - 1] - yValues[j]) *System.Math.Log(xiValues[i] / xValues[j]) /
                       System.Math.Log(xValues[j - 1] / xValues[j]);
                }
            }

            return yi;
        }

        #endregion

        #region Safe functions
        /// <summary>
        /// Safe Multiply. If the result of the multiplication causes an Integer overflow, an ArgumentException wil be thrown
        /// </summary>
        /// <param name="multiplicand"></param>
        /// <param name="multiplier"></param>
        /// <returns>multiplicand * multiplier</returns>
        public static int SafeMultiply(int multiplicand, int multiplier)
        {
            long multiplied = multiplicand * multiplier;
            if (multiplied > int.MaxValue)
                throw new ArgumentException("input value " + multiplicand + " multiplied by " + multiplier + " causes integer overflow");

            return (int)multiplied;
        }

        /// <summary>
        /// Safe Addition. If the result of the Addition causes an Integer overflow, an ArgumentException wil be thrown
        /// </summary>
        /// <param name="addend1"></param>
        /// <param name="addend2"></param>
        /// <returns>addend1 + addend2</returns>
        public static int SafeAdd(int addend1, int addend2)
        {
            long sum = addend1 + addend2;
            if (sum > int.MaxValue)
                throw new ArgumentException("input value " + addend1 + " + " + addend2 + "causes integer overflow");

            return (int)sum;
        }

        #endregion

        // FOR THE ONE WHO IS MERGING:
        // IF YOU EVER MERGE BACK IN THE METHOD:
        // 
        //         public static T[] Parse<T>(string s)
        //           where T : struct, IComparable, IConvertible, IComparable<T>, IEquatable<T>
        //           
        // JUST REMEMBER: IT IS LIKE PUTTING MAYONAISE ON MY HAMBURGER. I HATE MAYONAISE!
        // DON'T DO IT!!!!!!!
        // 
        // CONVERT ALL METHOD CALLS TO 
        // StringConverters.ParseToIntArray(string)
        // StringConverters.ParseToDoubleArray(string)
        // 
        // Eventually implement new parse methods!
    }
}
