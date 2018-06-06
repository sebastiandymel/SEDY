using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace FakeIMC.Math
{
    /// <summary>
    /// This Class is used to hold frequency value pairs, and get Interpolated values on the frequencies there is
    /// not present in the class.
    /// </summary>
    [Serializable] // Needed for debug visualizers
    public class Spectrum : ISpectrum
    {
        private VectorDouble frequencies;
        private VectorDouble values;

        #region Constructors

        public Spectrum()
            : this(0)
        { }

        public Spectrum(int size)
        {
            this.frequencies = new VectorDouble(size);
            this.values = new VectorDouble(size);
        }


        public Spectrum(IVectorDoubleReadOnly newFrequencies, IVectorDoubleReadOnly newValues)
        {
            if (newFrequencies.Length != newValues.Length)
                throw new ArgumentException("Frequencies and Values are not equal in size");

            this.frequencies = newFrequencies.Clone() as VectorDouble;
            this.values = newValues.Clone() as VectorDouble;
        }

        public Spectrum(ISpectrum spectrum)
            : this(spectrum.Frequencies, spectrum.Values)
        { }

        public Spectrum(double[][] spectrumData)
        {
            this.frequencies = new VectorDouble(spectrumData[0]);
            this.values = new VectorDouble(spectrumData[1]);
        }

        #endregion

        #region Private Functions


        /// <summary>
        /// Used to find the index of the previous frequency to the given frequency.
        /// if the frequency is contained in the list the previous is set to the index of the
        /// frequency and exact match is set to true.
        /// </summary>
        /// <param name="frequency">The frequency</param>
        /// <param name="previous">The index of previous frequency to given frequency</param>
        /// <param name="exactMatch">indicate if the frequency is in the list</param>
        private void FindNearestFrequencies(double frequency, int fromIndex, int toIndex, out int previous, out bool exactMatch)
        {
            int count = toIndex - fromIndex + 1;
            int center = count / 2 + fromIndex;
            double centerFreq = this.frequencies[center];

            if (count == 1 || centerFreq == frequency)
            {
                previous = center;
                exactMatch = (frequency == this.frequencies[previous]);
            }

            // Continue Search
            else
            {
                // [---  <-  ---] search left
                if (centerFreq > frequency)
                    FindNearestFrequencies(frequency, fromIndex, center - 1, out previous, out exactMatch);

                // [---  ->  ---] search right
                else
                    FindNearestFrequencies(frequency, center, toIndex, out previous, out exactMatch);
            }
        }

        /// <summary>
        /// Extend the spectrum to include the beginning and the end frequency.
        /// The values will be equal to the first and last element in the spectrum
        /// </summary>
        /// <param name="first">first frequency to extend the spectrum to</param>
        /// <param name="last">last frequency to extend the spectrum to</param>
        public void ExtendLimits(double first, double last)
        {
            if (Count > 0)
            {
                //If the first element in the spectrum is higher than the beginning,
                //insert new freq/val where freq equals beginning and the value is the same as the first element in the spectrum
                if (this.frequencies[0] > first)
                    Insert(first, this.values[0]);

                //If the last element in the spectrum is smaller than the end,
                //insert new freq/val where freq equals end and the value is the same as the last element in the spectrum
                if (this.frequencies[Count - 1] < last)
                    Insert(last, this.values[Count - 1]);
            }
        }

        private int FindInsertPoint(double frequency)
        {
            if (this.frequencies.Length == 0 || this.frequencies[0] > frequency)
                return 0;
            if (this.frequencies[this.frequencies.Length - 1] < frequency)
                return this.frequencies.Length;

            bool search = true;
            int low = 0;
            int high = Frequencies.Length - 1;
            int center = 0;
            double cur_freq = 0;
            while (search)
            {
                center = (low + high) / 2;
                cur_freq = this.frequencies[center];

                if (MathLib.IsEqualDoubles(cur_freq, frequency))
                    throw new ArgumentException("the specified frequency does already exist.", "frequency");
                else if (center == high)
                    search = false;
                else if (cur_freq < frequency)
                    low = center + 1;
                else if (cur_freq > frequency)
                    high = center - 1;
            }
            if (cur_freq < frequency)
                return center + 1;
            return center;
        }


        private void InsertAtPosition(int position, double frequency, double value)
        {
            this.values.InsertAt(position, value);
            this.frequencies.InsertAt(position, frequency);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Smoothes the spectrum 
        /// </summary>
        /// <param name="smoothFrequencies"></param>
        /// <param name="octav"></param>
        public void Smooth(IVectorDoubleReadOnly smoothFrequencies, double octave)
        {
            for (int i = 0; i < this.frequencies.Length; i++) this.values[i] = System.Math.Pow(10, this.values[i] / 20);

            VectorDouble val = new VectorDouble(smoothFrequencies.Length);

            double ru = System.Math.Pow(2, 1 / (2 * octave));
            double rl = 1.0 / ru;

            for (int k = 0; k < smoothFrequencies.Length; k++)
            {
                double fu = ru * smoothFrequencies[k];
                double fl = rl * smoothFrequencies[k];

                int meanCounter = 0;
                double meanValue = 0;

                for (int i = 0; i < this.frequencies.Length; i++)
                {
                    if (fl < this.frequencies[i] && this.frequencies[i] < fu)
                    {
                        meanValue += this.values[i];
                        meanCounter++;
                    }
                }

                val[k] = System.Math.Log10(meanValue / (double)meanCounter) * 20;
            }

            this.values = val;
            this.frequencies = new VectorDouble(smoothFrequencies);
        }

        /// <summary>
        /// Add all values in this spectrum with the (interpolated) values from source spectrum
        /// </summary>
        /// <param name="spectrum"></param>
        public virtual void Add(ISpectrum spectrum)
        {
            int length = this.frequencies.Length;

            for (int i = 0; i < length; i++)
            {
                double v = length == spectrum.Count && this.frequencies[i] == spectrum.Frequencies[i]
                               ? spectrum.Values[i]
                               : spectrum.GetValue(this.frequencies[i]);
                this.values[i] += v;
            }
        }

        public static ISpectrum Add(ISpectrum left, ISpectrum right)
        {
            Spectrum s = new Spectrum(left.Count);

            for (int i = 0; i < s.frequencies.Length; i++)
            {
                s.frequencies[i] = left.Frequencies[i];

                s.values[i] = left.Values[i] +
                               ((s.frequencies.Length == right.Count && s.frequencies[i] == right.Frequencies[i]) ?
                               right.Values[i] :
                               right.GetValue(s.frequencies[i]));

            }

            return s;

        }

        /// <summary>
        /// Subtract all values in this spectrum with the (interpolated) values from source spectrum
        /// </summary>
        /// <param name="spectrum"></param>
        public virtual void Subtract(ISpectrum spectrum)
        {
            int length = this.frequencies.Length;

            for (int i = 0; i < length; i++)
            {
                double v = length == spectrum.Count && this.frequencies[i] == spectrum.Frequencies[i]
                               ? spectrum.Values[i]
                               : spectrum.GetValue(this.frequencies[i]);
                this.values[i] -= v;
            }
        }

        /// <summary>
        /// Subtract all values in this spectrum with the (interpolated) values from source spectrum
        /// </summary>
        /// <param name="spectrum"></param>
        public static ISpectrum Subtract(ISpectrum left, ISpectrum right)
        {
            Spectrum s = new Spectrum(left.Count);

            for (int i = 0; i < s.frequencies.Length; i++)
            {
                s.frequencies[i] = left.Frequencies[i];

                s.values[i] = left.Values[i] -
                               ((s.frequencies.Length == right.Count && s.frequencies[i] == right.Frequencies[i]) ?
                               right.Values[i] :
                               right.GetValue(s.frequencies[i]));
            }

            return s;
        }

        /// <summary>
        /// Used to insert a frequency value pair in the ISpectrum
        /// </summary>
        /// <param name="frequency">The frequency</param>
        /// <param name="value">The value for the frequency</param>
        public virtual void Insert(double frequency, double value)
        {
            int position = FindInsertPoint(frequency);
            InsertAtPosition(position, frequency, value);
        }

        /// <summary>
        /// Remove a frequency value pair in the spectrum
        /// </summary>
        /// <param name="frequency">the frequncy to remove</param>
        /// <returns>true if the frequency is removed otherwhise false</returns>
        public virtual bool RemoveAt(double frequency)
        {
            FindNearestFrequencies(frequency, 0, Count - 1, out var index, out var match);
            if (match)
            {
                this.frequencies.RemoveAt(index);
                this.values.RemoveAt(index);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Convert the values in the source spectrum from dB to Linear
        /// This function does not modify own state but returns new Spectrum
        /// </summary>
        public static ISpectrum ConvertFromDecibelToLinear(ISpectrum source)
        {
            ISpectrum linear = new Spectrum(source);
            linear.ConvertFromDecibelToLinear();
            return linear;
        }

        /// <summary>
        /// Convert the values in the spectrum from dB to Linear
        /// This function modifies own state
        /// </summary>
        public virtual void ConvertFromDecibelToLinear()
        {
            for (int i = 0; i < this.values.Length; i++)
            {
                this.values[i] = MathLib.DecibelToLinear(Values[i]);
            }
        }

        /// <summary>
        /// Convert the values in the source spectrum from Linear to dB
        /// This function does not modify own state but returns new Spectrum
        /// </summary>
        public static ISpectrum ConvertFromLinearToDecibel(ISpectrum source)
        {
            ISpectrum dB = new Spectrum(source);
            dB.ConvertFromLinearToDecibel();
            return dB;
        }

        /// <summary>
        /// Convert the values in the spectrum from Linear to dB
        /// This function modifies own state
        /// </summary>
        public virtual void ConvertFromLinearToDecibel()
        {
            for (int i = 0; i < this.values.Length; i++)
            {
                this.values[i] = MathLib.LinearToDecibel(this.values[i]);
            }
        }

        /// <summary>
        /// Get the interpolated values for the matching frequencies. 
        /// The function calls the GetValue for each frequency in the vector
        /// </summary>
        /// <param name="frequency">A vector of frequencies</param>
        /// <returns>the values for the frequencies</returns>
        public virtual IVectorDoubleReadOnly GetValueList(IVectorDoubleReadOnly frequency)
        {
            VectorDouble result = new VectorDouble(frequency.Length);
            for (int i = 0; i < frequency.Length; i++)
                result[i] = GetValue(frequency[i]);

            return result;
        }

        /// <summary>
        /// Used to get the value for the current frequency. If the frequency is not present in
        /// the ISpectrum the value is interpolated
        /// </summary>
        /// <param name="frequency">The Frequency</param>
        /// <returns>The value for the given frequency</returns>
        public virtual double GetValue(double frequency)
        {
            return GetValue(frequency, InterpolateType.Logarithmic, SpectrumExtendMode.Flat, SpectrumExtendMode.Flat);
        }

        public virtual double GetValue(double frequency, InterpolateType interpolateType,
            SpectrumExtendMode lowFrequencyMethod, SpectrumExtendMode highFrequencyMethod)
        {
            if (this.frequencies.Length == 0)
                throw new ArgumentOutOfRangeException("frequency", "The spectrum is empty");

            double value;

            //Frequency is lower than the first
            if (frequency < Frequencies[0])
            {
                value = DoLFExtrapolation(frequency, interpolateType, lowFrequencyMethod);
            }
            //Frequency is higher than the last
            else if (frequency > Frequencies[Frequencies.Length - 1])
            {
                value = DoHFExtrapolation(frequency, interpolateType, highFrequencyMethod);
            }
            else
            {
                FindNearestFrequencies(frequency, 0, Count - 1, out int index, out bool exactMatch);

                //check if the frequency is already in the list and return the corosponding value
                if (exactMatch)
                    value = Values[index];
                else
                {
                    //Get the Interpolated value
                    if (interpolateType == InterpolateType.Logarithmic)
                        value = MathLib.LogInterpolate(Frequencies[index], Values[index], Frequencies[index + 1], Values[index + 1], frequency);

                    else if (interpolateType == InterpolateType.Linear)
                        value = MathLib.LinearInterpolate(Frequencies[index], Values[index], Frequencies[index + 1], Values[index + 1], frequency);

                    else
                        throw new NotImplementedException();
                }
            }
            return value;
        }

        private double DoLFExtrapolation(double frequency, InterpolateType interpolateType, SpectrumExtendMode lfMethod)
        {
            double result = 0;

            int index = 0;
            int previousIndex = 1;

            //LF Extrapolate
            if (lfMethod == SpectrumExtendMode.Extrapolate)
            {
                //Use logarithmic extrapolation
                if (interpolateType == InterpolateType.Logarithmic)
                    result = MathLib.LogInterpolate(Frequencies[previousIndex], Values[previousIndex], Frequencies[index], Values[index], frequency);
                //Use linear extrapolation
                else if (interpolateType == InterpolateType.Linear)
                    result = MathLib.LinearInterpolate(Frequencies[previousIndex], Values[previousIndex], Frequencies[index], Values[index], frequency);
            }
            //LF Extend flat
            else if (lfMethod == SpectrumExtendMode.Flat)
                result = Values[index];
            //No extrapolation
            else if (lfMethod == SpectrumExtendMode.None)
                throw new ArgumentOutOfRangeException("frequency", "The frequency is outside the bounds of spectrum, and Extend is set to None");
            else
                throw new NotImplementedException();

            return result;
        }

        public static bool AreEqual(ISpectrum spectrumExpected, ISpectrum spectrumTarget, double tolerance)
        {
            if (ReferenceEquals(spectrumExpected, spectrumTarget))
                return true;

            if (spectrumExpected.Count != spectrumTarget.Count)
                return false;

            for (int i = 0; i < spectrumExpected.Count; i++)
            {
                if (spectrumExpected.Frequencies[i] != spectrumTarget.Frequencies[i])
                    return false;

                if (!(System.Math.Abs(spectrumExpected.Values[i] - spectrumTarget.Values[i]) < tolerance))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare the length with the dimensions of another vector.
        /// </summary>
        /// <param name="spectrum">The spectrum to compare with.</param>
        /// <returns>True if the dimensions are equal, otherwise false.</returns>
        public bool CompareDimensions(ISpectrum spectrum)
        {
            return (Count == spectrum.Count);
        }

        public bool AreEqual(ISpectrum spectrumTarget, double tolerance)
        {
            return AreEqual(this, spectrumTarget, tolerance);
        }

        private double DoHFExtrapolation(double frequency, InterpolateType interpolateType, SpectrumExtendMode hfMethod)
        {
            double result = 0;

            int index = Frequencies.Length - 1;
            int previousIndex = Frequencies.Length - 2;

            //HF Extrapolate
            if (hfMethod == SpectrumExtendMode.Extrapolate)
            {
                //Use logarithmic extrapolation
                if (interpolateType == InterpolateType.Logarithmic)
                    result = MathLib.LogInterpolate(Frequencies[previousIndex], Values[previousIndex], Frequencies[index], Values[index], frequency);
                //Use linear extrapolation
                else if (interpolateType == InterpolateType.Linear)
                    result = MathLib.LinearInterpolate(Frequencies[previousIndex], Values[previousIndex], Frequencies[index], Values[index], frequency);
            }
            //HF Extend flat
            else if (hfMethod == SpectrumExtendMode.Flat)
                result = Values[index];
            //No extrapolation
            else if (hfMethod == SpectrumExtendMode.None)
                throw new ArgumentOutOfRangeException("frequency", "The frequency is outside the bounds of spectrum, and Extend is set to None");
            else
                throw new NotImplementedException();

            return result;
        }



        /// <summary>
        /// Set the value at a specific frequency. The frequency must exist.
        /// </summary>
        /// <param name="frequency">An existing frequency.</param>
        /// <param name="value">The new value at the specified frequency.</param>
        public virtual void SetValue(double frequency, double value)
        {
            FindNearestFrequencies(frequency, 0, Count - 1, out int previous, out bool exactMatch);

            if (!exactMatch)
                throw new ArgumentException("The supplied frequency does not exist.", "frequency");

            this.values[previous] = value;
        }

        /// <summary>
        /// Set the value at a specific frequency. If the frequency exists, returns true
        /// Otherwise returns false, but does not raise any exceptions.
        /// </summary>
        /// <param name="frequency">An existing frequency.</param>
        /// <param name="value">The new value at the specified frequency.</param>
        public void SetValueExistingFreqOnly(double frequency, double value)
        {
            FindNearestFrequencies(frequency, 0, Count - 1, out int previous, out bool exactMatch);

            if (exactMatch) this.values[previous] = value;
        }

        /// <summary>
        /// Clears the frequencies and values in the spectrum
        /// </summary>
        public virtual void Clear()
        {
            this.frequencies.Clear();
            this.values.Clear();
        }

        /// <summary>
        /// Indicates if the ISpectrum is empty or not.
        /// </summary>
        /// <returns>Returns true if the spectrum does not contain any values, otherwise false.</returns>
        public bool IsEmpty()
        {
            return (Count == 0);
        }

        /// <summary>
        /// Used to round the values in a ISpectrum with a defined number of decimals
        /// </summary>
        /// <param name="decimals">number of decimals</param>
        public virtual void Round(int decimals)
        {
            for (int i = 0; i < this.values.Length; i++)
            {
                this.values[i] = System.Math.Round(this.values[i], decimals, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Used to round the values in a ISpectrum
        /// </summary>
        public virtual void Round()
        {
            for (int i = 0; i < this.values.Length; i++)
            {
                this.values[i] = System.Math.Round(this.values[i], MidpointRounding.AwayFromZero);
            }
        }
        /// <summary>
        /// Calculates Average In Bands
        /// </summary>
        /// <param name="newFrequencies"></param>
        /// <param name="bandLimits"></param>
        /// <returns>new spectrum</returns>
        public virtual ISpectrum GetAverageInBandsSpectrum(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies)
        {
            //Convert spectrum to linear
            ISpectrum linearSpectrum = ConvertFromDecibelToLinear(this);

            //Do average in bands
            AlgorithmBase mean = new MeanAlgorithm();
            ISpectrum averagedSpectrum = mean.CalculateInBands(linearSpectrum, centerFrequencies, crossoverFrequencies);

            //Convert back to dB
            averagedSpectrum.ConvertFromLinearToDecibel();
            return averagedSpectrum;
        }

        public virtual ISpectrum PeakInBands(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies)
        {
            AlgorithmBase peak = new PeakAlgorithm();
            return peak.CalculateInBands(this, centerFrequencies, crossoverFrequencies);
        }

        public virtual ISpectrum DipInBands(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies)
        {
            AlgorithmBase dip = new DipAlgorithm();
            return dip.CalculateInBands(this, centerFrequencies, crossoverFrequencies);
        }

        /// <summary>
        /// Converts spectrum values from dB to power, sums the values into fewer bands and 
        /// converts the spectrum values back to dB.
        /// </summary>
        public virtual ISpectrum SumInBands(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies)
        {
            AlgorithmBase sum = new SumAlgorithm();
            return sum.CalculateInBands(this, centerFrequencies, crossoverFrequencies);
        }

        /// <summary>
        /// Returns a new spectrum with interpolated values at input parameter frequencies
        /// Uses default logarithmic interpolation and flat extrapolation in LF and HF
        /// </summary>
        public static ISpectrum Interpolate(IVectorDoubleReadOnly frequencies, ISpectrum source)
        {
            return Interpolate(frequencies, source, InterpolateType.Logarithmic, SpectrumExtendMode.Flat, SpectrumExtendMode.Flat);
        }

        /// <summary>
        /// Returns a new Spectrum with interpolated values at input parameter frequencies. Allows specification of interpolation and extrapolation types.
        /// LF Method is the extrapolation method in low frequencies and HF Method is the extrapolation method in high frequencies
        /// </summary>        
        public static ISpectrum Interpolate(IVectorDoubleReadOnly frequencies, ISpectrum source, InterpolateType interpolateType,
            SpectrumExtendMode lowFrequencyMethod, SpectrumExtendMode highFrequencyMethod)
        {
            Spectrum result = new Spectrum();
            result.frequencies = frequencies.Clone() as VectorDouble;
            result.values = new VectorDouble(frequencies.Length);

            VectorDouble vector = result.values;
            for (int i = 0; i < frequencies.Length; i++)
            {
                vector[i] = source.GetValue(frequencies[i], interpolateType, lowFrequencyMethod, highFrequencyMethod);
            }
            return result;
        }


        /// <summary>
        /// Interpolate the value at the given frequency, and insert the value
        /// in the spectrum. The function ignores frequencies for which it
        /// already holds values.
        /// </summary>
        /// <param name="frequency">the frequency to interpolate and insert</param>
        public virtual void InterpolateAndInsert(double frequency)
        {
            if (!HasValueAt(frequency))
                Insert(frequency, GetValue(frequency));
        }

        /// <summary>
        /// Interpolate the values at the given frequencies, and insert the
        /// values in the spectrum. The function ignores frequencies for which
        /// it already holds values.
        /// </summary>
        /// <param name="frequencies">the frequencies to interpolate and insert</param>
        public virtual void InterpolateAndInsert(IVectorDoubleReadOnly newFrequencies)
        {
            for (int i = 0; i < newFrequencies.Length; i++)
                InterpolateAndInsert(newFrequencies[i]);
        }

        /// <summary>
        /// Check if the spectrum cotains a value on a given frequenzy
        /// </summary>
        /// <param name="frequenzy">a frequenzy</param>
        /// <returns>true if the spectrum has a value on the given frequenzy otherwhise false</returns>
        public virtual bool HasValueAt(double frequency)
        {
            if (this.frequencies.Length == 0)
                return false;

            int low = 0;
            int high = this.frequencies.Length - 1;

            while (true)
            {
                int index = (high + low) / 2;
                if (this.frequencies[index] == frequency)
                    return true;
                if (low >= high)
                    return false;
                if (this.frequencies[index] < frequency)
                    low = index + 1;
                else if (this.frequencies[index] > frequency)
                    high = index - 1;
            }
        }

        /// <summary>
        /// Cuts the end of a spectrum. The spectrum will include the frequency 
        /// from the parameter, if it is present in the spectrum. Otherwise it
        /// cutoff will happen at the frequency nearest to the parameter
        /// </summary>
        /// <param name="frequency">Frequency where the spectrum is cut off</param>
        public void CutEnd(double frequency)
        {
            if (frequency < this.frequencies[Count - 1])
            {
                FindNearestFrequencies(frequency, 0, Count - 1, out int index, out bool match);
                this.frequencies.Resize(index + 1);
                this.values.Resize(index + 1);
            }
        }

        /// <summary>
        /// Sets the start of a spectrum. The spectrum will include the frequency 
        /// from the parameter, if it is present in the spectrum. Otherwise the
        /// cutoff will happen at the frequency nearest to the parameter
        /// </summary>
        /// <param name="frequency">Frequency where the spectrum should start</param>
        public void SetStart(double frequency)
        {
            if (frequency < this.frequencies[Count - 1])
            {
                FindNearestFrequencies(frequency, 0, Count - 1, out int index, out bool match);
                for (int i = 0; i < index; i++)
                {
                    this.frequencies.RemoveAt(0);
                    this.values.RemoveAt(0);
                }
            }
        }


        #endregion

        #region Equals & GetHashCode

        /// <summary>
        /// This Function is used to check if the object is equal the to this object
        /// </summary>
        /// <param name="obj">spectrum to check on</param>
        /// <returns>true if the obejct are equals</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Spectrum spectrum = obj as Spectrum;

            if (ReferenceEquals(this, spectrum))
                return true;

            if (Count != spectrum.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (!MathLib.IsEqualDoubles(this.frequencies[i], spectrum.frequencies[i]))
                    return false;
                if (!MathLib.IsEqualDoubles(this.values[i], spectrum.values[i]))
                    return false;
            }

            return true;
        }


        // override object.GetHashCode
        public override int GetHashCode()
        {
            if (this.values.Length == 0)
                return 0;
            return this.values.Length ^ (int) this.values[0];
        }

        #endregion

        #region Properties and Fields

        /// <summary>
        /// Used to get a vector of the values in the spectrum
        /// </summary>
        public IVectorDoubleReadOnly Values
        {
            get { return this.values; }
            protected set { this.values = value as VectorDouble; }
        }

        /// <summary>
        /// Used to get a readonly vector of the frequencies in spectrum
        /// </summary>
        public IVectorDoubleReadOnly Frequencies
        {
            get { return this.frequencies; }
        }

        /// <summary>
        /// Return the count of frequency value pairs in the spectrum
        /// </summary>
        public int Count
        {
            get { return this.frequencies.Length; }
        }

        public double this[int index]
        {
            get { return this.values[index]; }
            set { this.values[index] = value; }
        }

        #endregion

        #region ICloneable Members

        public ISpectrum Clone()
        {
            return new Spectrum(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IXmlSerializable Members

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            const string FrequencyAttributeName = "Frequency";

            if (reader == null)
                throw new ArgumentNullException("reader");

            Clear();

            if (!reader.IsEmptyElement) //Avoid reading too far if we just have an empty spectrum element
            {
                reader.Read(); //Read Spectrum start element
                while (reader.IsStartElement())
                {
                    var frequency = XmlConvert.ToDouble(reader.GetAttribute(FrequencyAttributeName));
                    var value = reader.ReadElementContentAsDouble();
                    Insert(frequency, value);
                }
            }
            reader.Read(); //Read Spectrum end element
        }


        public virtual void WriteXml(XmlWriter writer)
        {
            for (int i = 0; i < this.frequencies.Length; i++)
            {
                writer.WriteStartElement("Value");
                writer.WriteAttributeString("Frequency", XmlConvert.ToString(this.frequencies[i]));
                writer.WriteValue(this.values[i]);
                writer.WriteEndElement();
            }
        }

        #endregion

        #region Nested helper classes

        private abstract class AlgorithmBase
        {
            protected abstract double Calculate(ISpectrum inputSpectrum, int lowerIndex, int upperIndex);

            /// <summary>
            /// This method is used to create a new spectrum where mean or peak values are calculated in all bands
            /// </summary>
            /// <param name="source">spectrum that contains frequencies and values</param>
            /// <param name="newFrequencies">Centerfrequencies for the bands</param>
            /// <param name="bandLimits">Crossover frequencies for the bands</param>
            /// <returns>new spectrum containing new frequencies and values</returns>

            public ISpectrum CalculateInBands(ISpectrum source, IVectorDoubleReadOnly newFrequencies, IVectorDoubleReadOnly bandLimits)
            {
                // $Archive: /Matlas/All/Common/@CDResponse/AvInBands.m $ 
                // $Revision: 11 $  $Author: Str_aht $  $Date: 11-04-06 15:39 $

                //Create new spectrum to hold the new frequencies and values
                ISpectrum outputSpectrum = new Spectrum();

                for (int i = 0; i < newFrequencies.Length; i++)
                {
                    int inx1 = FindIndexOfLowerLimit(source, bandLimits[i]);
                    int inx2 = FindIndexOfUpperLimit(source, bandLimits[i + 1]);

                    double value;

                    if (inx1 == -1)
                        value = source.Values.Last;
                    else if (inx2 == -1)
                        value = source.Values.First;
                    else if (inx1 > inx2)
                        value = MathLib.Interpolate(
                            source.Frequencies,
                            source.Values,
                            newFrequencies[i],
                            InterpolateType.Linear);
                    else
                        value = Calculate(source, inx1, inx2);

                    outputSpectrum.Insert(newFrequencies[i], value);
                }

                // Extend the resulting spectrum
                outputSpectrum.ExtendLimits(newFrequencies[0], newFrequencies[newFrequencies.Length - 1]);

                return outputSpectrum;
            }

            private static int FindIndexOfUpperLimit(ISpectrum source, double upperLimit)
            {
                int upperIndex = -1;

                for (int j = 0; j < source.Frequencies.Length; j++)
                {
                    if (upperLimit >= source.Frequencies[j])
                        upperIndex = j;
                    else
                        break;
                }
                return upperIndex;
            }

            private static int FindIndexOfLowerLimit(ISpectrum source, double lowerLimit)
            {
                for (int j = 0; j < source.Frequencies.Length; j++)
                {
                    if (lowerLimit <= source.Frequencies[j])
                        return j;

                }
                return -1;
            }

        }

        private class MeanAlgorithm : AlgorithmBase
        {
            protected override double Calculate(ISpectrum inputSpectrum, int lowerIndex, int upperIndex)
            {
                int count = upperIndex - lowerIndex + 1;
                return inputSpectrum.Values.Mean(lowerIndex, count);
            }
        }

        private class PeakAlgorithm : AlgorithmBase
        {

            protected override double Calculate(ISpectrum inputSpectrum, int lowerIndex, int upperIndex)
            {
                double peakValue = inputSpectrum[lowerIndex];

                for (int i = lowerIndex; i <= upperIndex && i < inputSpectrum.Count; i++)
                {
                    double d = inputSpectrum[i];
                    if (peakValue < d)
                        peakValue = d;
                }
                return peakValue;
            }
        }

        private class DipAlgorithm : AlgorithmBase
        {
            protected override double Calculate(ISpectrum inputSpectrum, int lowerIndex, int upperIndex)
            {
                double peakValue = inputSpectrum[lowerIndex];
                int inputSpectrumSize = inputSpectrum.Count;
                for (int i = lowerIndex; i <= upperIndex && i < inputSpectrumSize; i++)
                {
                    double d = inputSpectrum[i];
                    if (peakValue > d)
                        peakValue = d;
                }
                return peakValue;
            }
        }

        /// <summary>
        /// Converts spectrum values from dB to power, sums the values to fewer bands and convert back to dB.
        /// </summary>
        private class SumAlgorithm : AlgorithmBase
        {
            protected override double Calculate(ISpectrum inputSpectrum, int lowerIndex, int upperIndex)
            {
                // Test input arguments
                if (upperIndex >= inputSpectrum.Count)
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "'upperIndex' must not exceed dimesions of 'inputSpectrum'."));

                // Convert from db to power. Sum the bands. Convert back to dB.
                double sumPower = 0.0;
                for (int i = lowerIndex; i <= upperIndex; i++)
                {
                    double toPower =System.Math.Pow(10, inputSpectrum[i] / 10.0); // Input dB to power
                    sumPower += toPower;
                }

                // Avoid zeros because log10 of zero gives -INF.
                if (sumPower == 0)
                    sumPower = 0.00001;

                return System.Math.Log10(sumPower) * 10.0;// Convert back to dB.
            }
        }

        #endregion

        #region Static functions
        /// <summary>
        /// Returns wether the suppliend ISpectrum is null or empty.
        /// </summary>
        /// <param name="obj">The ISpectrum in question.</param>
        /// <returns>Returns true if the ISpectrum is null or empty, otherwise false.</returns>
        public static bool IsNullOrEmpty(ISpectrum spectrum)
        {
            if (spectrum == null || spectrum.IsEmpty())
                return true;
            return false;
        }

        public static void Smooth(ISpectrum spectrum, IVectorDoubleReadOnly frequency, double octave)
        {
            spectrum.Smooth(frequency, octave);
        }

        #endregion

        /// <summary>
        /// Returns a string which represent the frequencies and values of the
        /// ISpectrum / IAudiogram.
        /// </summary>
        /// <returns>A string representation of the ISpectrum / IAudiogram.</returns>
        public override string ToString()
        {
            return "frequencies {" + this.frequencies.ToString() + "}, values {" + this.values.ToString() + "}";
        }
    }
}
