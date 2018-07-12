using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace FakeIMC.Math
{
    /// <summary>Methods for extending the generic IVector interface, mainly regarding index searching.</summary>
    public static class VectorTExtensions
    {
        /// <summary>Checks if the value passed as argument exists in the Vector. If it exists true is returned
        /// and otherwise false.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector in which the value should be found.</param>
        /// <param name="value">The value to check the existance of.</param>
        /// <returns>Returns true if the value passed as argument is found in the Vector, otherwise false.</returns>
        public static bool Exists<T>(this IVector<T> vector, T value)
            where T : struct, IComparable<T>
        {
            return FirstIndex<T>(vector, x => x.CompareTo(value) == 0) != -1;
        }

        /// <summary>Checks if the expression passed as argument evaluates to any entries in the Vector. If it values 
        /// are found using this expression true is returned and otherwise false.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector in which the values should be found.</param>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Returns true if expression evaluates to any values in the Vector, otherwise false.</returns>
        public static bool Exists<T>(this IVector<T> vector, Func<T, bool> expression)
            where T : struct, IComparable<T>
        {
            return FirstIndex<T>(vector, expression) != -1;
        }

        /// <summary>Finds the first index in the vector where value is matched.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector to search.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>The index of the first match, and -1 if no matches were found.</returns>
        public static int FirstIndex<T>(this IVector<T> vector, T value)
            where T : struct, IComparable<T>
        {
            return FirstIndex<T>(vector, x => x.CompareTo(value) == 0);
        }

        /// <summary>Finds the first index in the vector for which the expression evaluates true.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector to search.</param>
        /// <param name="expression">The match expression.</param>
        /// <returns>The index of the first match, and -1 if no matches were found.</returns>
        public static int FirstIndex<T>(this IVector<T> vector, Func<T, bool> expression)
            where T : struct, IComparable<T>
        {
            int[] result = FindIndexes<T>(vector, true, false, expression);

            if (result.Length == 0)
                return -1;

            return result[0];
        }

        /// <summary>Finds the last index in the vector where value is matched.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector to search.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>The index of the last match, and -1 if no matches were found.</returns>
        public static int LastIndex<T>(this IVector<T> vector, T value)
            where T : struct, IComparable<T>
        {
            return LastIndex<T>(vector, x => x.CompareTo(value) == 0);
        }

        /// <summary>Finds the last index in the vector where value is matched.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector to search.</param>
        /// <param name="expression">The match expression.</param>
        /// <returns>The index of the last match, and -1 if no matches were found.</returns>
        public static int LastIndex<T>(this IVector<T> vector, Func<T, bool> expression)
            where T : struct, IComparable<T>
        {
            int[] result = FindIndexes<T>(vector, true, true, expression);

            if (result.Length == 0)
                return -1;

            return result[0];
        }

        /// <summary>Finds all indices in the vector where the value passed as argument is matched.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector to search.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>The indices of all matches, and the empty array if no matches were found.</returns>
        public static int[] AllIndexes<T>(this IVector<T> vector, T value)
            where T : struct, IComparable<T>
        {
            return AllIndexes<T>(vector, x => x.CompareTo(value) == 0);
        }

        /// <summary>Finds all indices in the vector where the expression passed as argument evaluates true.</summary>
        /// <typeparam name="T">The type of the values to match.</typeparam>
        /// <param name="vector">The vector to search.</param>
        /// <param name="expression">The match expression.</param>
        /// <returns>All indices that match the expression passed as argument, and the empty array if no matches found.</returns>
        public static int[] AllIndexes<T>(this IVector<T> vector, Func<T, bool> expression)
            where T : struct, IComparable<T>
        {
            return FindIndexes<T>(vector, false, false, expression);
        }

        /// <summary>Finds all indices where the search predicate matches. E.g. Finds all indices where source is lesser 
        /// than the same position in the target vector.</summary>
        /// <param name="source">The source vector.</param>
        /// <param name="target">The target vector to match up against.</param>
        /// <param name="expression">The match expression.</param>
        /// <returns>Returns an int array of all matched indices.</returns>
        public static int[] AllIndexes<T>(this IVector<T> source, IVector<T> target, Func<T, T, bool> expression)
            where T : struct, IComparable<T>
        {
            return FindIndexes(source, target, expression);
        }

        #region Private Parts

        private static int[] FindIndexes<T>(IVector<T> vector, bool stopOnMatch, bool reverse, Func<T, bool> expression)
            where T : struct, IComparable<T>
        {
            List<int> results = new List<int>(vector.Length);

            int length = vector.Length;
            if (reverse)
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    if (expression(vector[i]))
                    {
                        if (stopOnMatch)
                            return new int[] { i };
                        else
                            results.Add(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    if (expression(vector[i]))
                    {
                        if (stopOnMatch)
                            return new int[] { i };
                        else
                            results.Add(i);
                    }
                }
            }

            return results.ToArray();
        }

        private static int[] FindIndexes<T>(IVector<T> source, IVector<T> target, Func<T, T, bool> expression)
            where T : struct, IComparable<T>
        {
            if (source.Length != target.Length)
                throw new ArgumentException("The dimension of the vector does not match the source.", "target");

            List<int> results = new List<int>(source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                if (expression(source[i], target[i]))
                    results.Add(i);
            }

            return results.ToArray();
        }

        #endregion
    }
}
