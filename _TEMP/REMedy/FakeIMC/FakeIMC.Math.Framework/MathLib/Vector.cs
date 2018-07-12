using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace FakeIMC.Math
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "We do not want IVector<T> to end in 'Collection' because that would sound like a collection of vectors")]
    public class Vector<T> : IVector<T>
        where T : struct, IComparable<T>
    {
        protected interface IVectorHelpers
        {
            T[] GetDataArray(int neededSize);
            void CopyFrom(T[] source, T[] target, int count);
        }

        const int MinArraySize = 8;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields",
            Justification = "In this case we want to have direct member access without a collection property for performance reasons.")]
        protected T[] data;

        public T[] GetData() { return this.data; }

        public int Length { get; protected set; }

        public Vector()
            : this(new VectorHelpersGeneric(), 0)
        {
        }

        public Vector(int length)
            : this(new VectorHelpersGeneric(), length)
        {
        }
        private readonly IVectorHelpers vectorHelpers;

        protected Vector(IVectorHelpers helpers, int length)
        {
            this.vectorHelpers = helpers;
            Create(length);
        }

        public Vector(int length, T value)
            : this(new VectorHelpersGeneric(), length)
        {
            Initialize(value);
        }

        public Vector(T[] values)
            : this(new VectorHelpersGeneric(), values)
        { }


        protected Vector(IVectorHelpers helpers, T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            this.vectorHelpers = helpers;
            Length = values.Length;
            this.data = new T[Length];
            helpers.CopyFrom(values, this.data, values.Length);
        }

        public Vector(Vector<T> vector)
            : this(vector, new VectorHelpersGeneric())
        {
        }

        protected Vector(Vector<T> vector, IVectorHelpers helpers)
        {
            this.vectorHelpers = helpers;
            Length = vector.Length;
            this.data = new T[Length];
            helpers.CopyFrom(vector.data, this.data, vector.Length);
        }

        public Vector(IVector<T> vector)
            : this((Vector<T>)vector)
        { }

        protected void Create(int newLength)
        {
            this.data = this.vectorHelpers.GetDataArray(newLength);
            Length = newLength;
        }

        /// <summary>
        /// Initialize all values of the vector to be the supplied.
        /// </summary>
        /// <param name="value">Initialize value.</param>
        public void Initialize(T value)
        {
            for (int i = 0; i < Length; i++)
                this.data[i] = value;
        }

        public void Initialize(params T[] value)
        {
            this.data = value;
            Length = value.Length;
        }

        /// <summary>
        /// Initializes the vector with values from another vector. If the two vectors have different dimensions, a new vector will be created.
        /// </summary>
        /// <param name="vector">Vector with values for initialization.</param>
        public void Initialize(Vector<T> vector)
        {
            Resize(vector.Length);
            CopyFrom(vector.data);
        }

        public void Replace(T source, T destination)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i].CompareTo(source) == 0) this.data[i] = destination;
            }
        }

        /// <summary>
        /// Remove all elements from the vector. Leaves the internal size intact.
        /// </summary>
        public virtual void Clear()
        {
            Resize(0);
        }

        [Serializable]
        protected class VectorHelpersGeneric : IVectorHelpers
        {
            /// <summary>
            /// Calculates an appropriate size of the internal array based on the needed size
            /// </summary>
            /// <param name="neededSize">The size needed</param>
            /// <returns>a size to use for the internal array</returns>
            public T[] GetDataArray(int neededSize)
            {
                int size = Vector<T>.MinArraySize;
                while (size < neededSize)
                    size <<= 1;

                return new T[size];
            }

            public void CopyFrom(T[] source, T[] target, int count)
            {
                if (source == null || target == null)
                    return;

                for (int i = 0; i < count; i++)
                    target[i] = source[i];
            }
        }



        public virtual void CopyFrom(T[] source, int count)
        {
            this.vectorHelpers.CopyFrom(source, this.data, count);
        }

        public virtual void CopyFrom(T[] source)
        {
            if (source == null)
                return;

            CopyFrom(source,System.Math.Min(source.Length, Length));
        }

        public virtual void Copy(T[] source, int sourceIndex, T[] destination, int destinationIndex, int count)
        {
            if (source == null || destination == null)
                return;

            for (int i = 0; i < count; i++)
                destination[i + destinationIndex] = source[i + sourceIndex];
        }

        public virtual void MoveLeft(int index, int count)
        {
            if (count < 0)
                throw new ArgumentException("Count cannot be a negative number.", "count");

            for (int i = 0; i < count; i++) this.data[i + index - 1] = this.data[i + index];
        }

        public virtual void MoveRight(int index, int count)
        {
            if (count < 0)
                throw new ArgumentException("Count cannot be a negative number.", "count");

            for (int i = count - 1; i >= 0; i--) this.data[i + index + 1] = this.data[i + index];
        }

        public void Swap(int index1, int index2)
        {
            T t = this.data[index1];
            this.data[index1] = this.data[index2];
            this.data[index2] = t;
        }

        /// <summary>
        /// Rezises the vector to a new length. 
        /// </summary>
        /// <param name="size">The new length of the vector</param>
        /// <param name="copy">True if old values should be preserved</param>
        /// <returns>True if the internal data array was reallocated</returns>
        private bool Resize(int size, bool copy)
        {
            bool resized = false;

            if (this.data == null)
            {
                if (size != 0)
                {
                    this.data = this.vectorHelpers.GetDataArray(size);
                    resized = true;
                }
            }

            else if (size > this.data.Length)
            {
                T[] tmp = this.vectorHelpers.GetDataArray(size);

                if (copy)
                    Copy(this.data, 0, tmp, 0, Length);

                this.data = tmp;
                resized = true;
            }

            Length = size;

            return resized;
        }

        public void Resize(int size)
        {
            Resize(size, true);
        }

        /// <summary>
        /// Rezises and inserts a value at a given position in the vector
        /// 
        /// DO NOT use this function if the array is contained in a
        /// MatrixDouble.
        /// </summary>
        /// <param name="index">The index to insert at</param>
        /// <param name="value">The value to be inserted</param>
        public void InsertAt(int index, T value)
        {
            if (index == int.MaxValue)
                throw new ArgumentException("Index must be less than int.MaxValue", "index");

            T[] oldData = this.data;

            if (Resize(Length + 1, false))
            {
                if (oldData != null && oldData.Length > 0)
                {
                    Copy(oldData, 0, this.data, 0, index);
                    if (index < oldData.Length)
                        Copy(oldData, index, this.data, index + 1, Length - index - 1);
                }
            }
            else
            {
                MoveRight(index, Length - index - 1);
            }

            this.data[index] = value;
        }

        /// <summary>
        /// Remove a variable in the Vector
        /// 
        /// DO NOT use this function if the array is contained in a
        /// MatrixDouble.
        /// </summary>
        /// <param name="index">the index of the variable to remove</param>
        public void RemoveAt(int index)
        {
            if (index != int.MaxValue)
                MoveLeft(index + 1, Length - index - 1);
            Resize(Length - 1);
        }

        /// <summary>
        /// Appends an element on the end of the vector initialized to 0.0.
        /// 
        /// DO NOT use this function if the array is contained in a
        /// MatrixDouble.
        /// </summary>
        public void Append()
        {
            Resize(Length + 1);
        }

        /// <summary>
        /// Appends an element on the end of the vector initialized to parameter
        /// 'value'.
        /// 
        /// DO NOT use this function if the array is contained in a
        /// MatrixDouble.
        /// </summary>
        /// <param name="value">The value to be appended.</param>
        public void Append(T value)
        {
            Resize(Length + 1);
            this.data[Length - 1] = value;
        }

        /// <summary>
        /// Appends a number of elements to the end of the vector initialized to
        /// the parameter 'value'.
        /// 
        /// DO NOT use this function if the array is contained in a
        /// MatrixDouble.
        /// </summary>
        /// <param name="count">The number of new elements to append to the vector.</param>
        /// <param name="value">The value for the appended values.</param>
        public void Append(int count, T value)
        {
            Resize(Length + count);
            for (int i = Length - count; i < Length; i++) this.data[i] = value;
        }

        /// <summary>
        /// Appends a vector of values to the end of the vector.
        /// 
        /// DO NOT use this function if the array is contained in a
        /// MatrixDouble.
        /// </summary>
        /// <param name="vector">Vector with values to append.</param>
        public void Append(IVector<T> vector)
        {
            int l = Length;
            Resize(Length + vector.Length, true);
            Copy(((Vector<T>)vector).data, 0, this.data, l, vector.Length);
        }

        public void Set(IVector<T> vector, int destinationIndex, int count)
        {
            Copy(((Vector<T>)vector).data, 0, this.data, destinationIndex,System.Math.Min(vector.Length, count));
        }

        public void Set(IVector<T> vector, int destinationIndex)
        {
            Set(vector, destinationIndex, vector.Length);
        }

        /// <summary>
        /// Merges new vector into the current vector, method omits repeated values. It sorts the vector at the end.
        /// </summary>
        /// <param name="vector">Vector with values to append.</param>
        public void Merge(IVector<T> vector)
        {
            if (vector == null)
                return;

            foreach (T t in vector)
            {
                if (!Contains(t))
                    Append(t);
            }
            Sort();
        }

        /// <summary>
        /// Sorts vector using quick sort algorithm
        /// </summary>
        /// <returns></returns>
        private void QuickSort(int p, int r)
        {
            if (p < r)
            {
                T x = this.data[r];
                int i = p - 1;
                for (int j = p; j < r; j++)
                {
                    if (this.data[j].CompareTo(x) <= 0)
                    {
                        i++;
                        T t = this.data[i];
                        this.data[i] = this.data[j];
                        this.data[j] = t;
                    }
                }

                T tmp = this.data[i + 1];
                this.data[i + 1] = this.data[r];
                this.data[r] = tmp;
                int q = i + 1;

                QuickSort(p, q - 1);
                QuickSort(q + 1, r);
            }
        }

        /// <summary>
        /// Sorts elements of vector in ascending order
        /// </summary>
        /// <returns>The sorted vector</returns>
        public void Sort()
        {
            if (Length == 1)
            {
                return;
            }
            // TODO how often?????
            if (Length == 2)
            {
                if (this.data[0].CompareTo(this.data[1]) > 0)
                    Swap(0, 1);
            }
            else if (Length == 3)
            {
                if (this.data[0].CompareTo(this.data[2]) > 0)
                    Swap(0, 2);

                if (this.data[1].CompareTo(this.data[0]) < 0)
                    Swap(1, 0);
                else if (this.data[1].CompareTo(this.data[2]) > 0)
                    Swap(1, 2);
            }
            else
            {
                QuickSort(0, Length - 1);
            }
        }

        /// <summary>
        /// See if the vector contanins a value
        /// </summary>
        /// <param name="value">The value to find</param>
        /// <returns>True if the value is found, otherwise false.</returns>
        public bool Contains(T value)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this.data[i].CompareTo(value) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Flip the values of the vector from left to right.
        /// </summary>
        public void Flip()
        {
            Array.Reverse(this.data, 0, Length);
        }

        public T this[int index]
        {
            get
            {
                // We need to check the index since the internal data
                // representation could be larger than the vector length.
                if (index >= Length)
                    throw new ArgumentException("index must be within the range of the vector.", "index");

                return this.data[index];
            }
            set
            {
                // We need to check the index since the internal data
                // representation could be larger than the vector length.
                if (index >= Length)
                    throw new ArgumentException("index must be within the range of the vector.", "index");

                this.data[index] = value;
            }
        }

        public bool IsEmpty
        {
            get { return Length == 0; }
        }

        /// <summary>
        /// Get or set the first element of the vector
        /// </summary>
        public T First
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        /// <summary>
        /// Get or set the last element of the vector.
        /// </summary>
        public T Last
        {
            get { return this[Length - 1]; }
            set { this[Length - 1] = value; }
        }

        public virtual object Clone()
        {
            return new Vector<T>(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return this.data[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsLengthEqual(IVector<T> other)
        {
            return other.Length == Length;
        }

        public virtual bool IsEqual(IVector<T> other)
        {
            if (!IsLengthEqual(other))
                return false;

            for (int i = 0; i < Length; i++)
                if (other[i].CompareTo(this.data[i]) != 0)
                    return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            IVector<T> v = obj as IVector<T>;

            if (v == null)
                return false;

            return IsEqual(v);
        }

        /// <summary>
        /// If the hash codes of two object values are different, the object
        /// values are guaranteed to be different. However, if the hash codes of
        /// two object values are the same, the object values are not guaranteed
        /// to be the same. An additional call to Object.Equals() must be made
        /// to confirm that the object values are the same.
        /// </summary>
        /// <returns>The HashCode corresponding to the object.</returns>
        public override int GetHashCode()
        {
            return (Length > 0) ? this.data[Length / 2].GetHashCode() : 0;
        }

        public T Min()
        {
            if (Length <= 0)
                throw new InvalidOperationException("Vector is empty.");

            T value = this.data[0];
            for (int i = 1; i < Length; i++)
            {
                if (this.data[i].CompareTo(value) < 0)
                    value = this.data[i];
            }
            return value;
        }

        public T Max()
        {
            if (Length <= 0)
                throw new InvalidOperationException("Vector is empty.");

            T value = this.data[0];
            for (int i = 1; i < Length; i++)
            {
                if (this.data[i].CompareTo(value) > 0)
                    value = this.data[i];
            }
            return value;
        }

        virtual public IVector<T> GetValues(int index, int count)
        {
            if (index >= Length)
                throw new ArgumentException("Supplied index must be a valid index within the vector.", "index");
            if (count > Length)
                throw new ArgumentException("Count cannot be larger than the vector length.", "count");

            Vector<T> v = new Vector<T>(count);
            for (int i = 0; i < count; i++)
                v[i] = this[index + i];

            return v;
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Xml serialization element and attribut names
        /// </summary>
        private const String XmlElementValue = "Value";
        private const String XmlAttributeIndex = "index";

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads a list of 'Value' nodes. 
        /// The cursor must point to the parent node of 'Value' nodes.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            reader.ReadStartElement();

            // Read all elements to temporary list
            LinkedList<T> tempList = new LinkedList<T>();
            while (reader.LocalName == Vector<T>.XmlElementValue)
            {
                T element = (T)reader.ReadElementContentAs(typeof(T), null);
                tempList.AddLast(element);
            }

            // Make sure we have enough room
            if (tempList.Count > Length) this.data = new T[tempList.Count];

            // And transfer the data to the internal array
            tempList.CopyTo(this.data, 0);

            Length = tempList.Count;

            reader.ReadEndElement();
        }

        /// <summary>
        /// Writes a number of 'Value' elements to the writer corresponding to the number of elements in the vector.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            for (int index = 0; index < Length; index++)
            {
                writer.WriteStartElement(Vector<T>.XmlElementValue);
                writer.WriteAttributeString(Vector<T>.XmlAttributeIndex, XmlConvert.ToString(index));
                writer.WriteValue(this.data[index]);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
