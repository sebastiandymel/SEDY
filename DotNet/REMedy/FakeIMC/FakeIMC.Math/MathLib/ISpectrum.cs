using System;
using System.Xml.Serialization;

namespace FakeIMC.Math
{
    public interface ISpectrum : ICloneable, IXmlSerializable
    {
        void Add(ISpectrum spectrum);
        bool AreEqual(ISpectrum spectrumTarget, double tolerance);
        void Clear();
        new ISpectrum Clone();
        bool CompareDimensions(ISpectrum spectrum);
        void ConvertFromDecibelToLinear();
        void ConvertFromLinearToDecibel();
        int Count { get; }
        void CutEnd(double frequency);
        ISpectrum DipInBands(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "If it's good enough for Microsoft's help file, it's good enough for me.")]
        bool Equals(object obj);
        void ExtendLimits(double first, double last);
        IVectorDoubleReadOnly Frequencies { get; }
        ISpectrum GetAverageInBandsSpectrum(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies);
        int GetHashCode();
        new System.Xml.Schema.XmlSchema GetSchema();
        double GetValue(double frequency);
        double GetValue(double frequency, InterpolateType interpolateType, SpectrumExtendMode lowFrequencyMethod, SpectrumExtendMode highFrequencyMethod);
        IVectorDoubleReadOnly GetValueList(IVectorDoubleReadOnly frequency);
        bool HasValueAt(double frequency);
        void Insert(double frequency, double value);
        void InterpolateAndInsert(double frequency);
        void InterpolateAndInsert(IVectorDoubleReadOnly newFrequencies);
        bool IsEmpty();
        ISpectrum PeakInBands(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies);
        new void ReadXml(System.Xml.XmlReader reader);
        bool RemoveAt(double frequency);
        void Round();
        void Round(int decimals);
        void SetStart(double frequency);
        void SetValue(double frequency, double value);
        void SetValueExistingFreqOnly(double frequency, double value);
        void Smooth(IVectorDoubleReadOnly smoothFrequencies, double octave);
        void Subtract(ISpectrum spectrum);
        ISpectrum SumInBands(IVectorDoubleReadOnly centerFrequencies, IVectorDoubleReadOnly crossoverFrequencies);
        double this[int index] { get; set; }
        string ToString();
        IVectorDoubleReadOnly Values { get; }
        new void WriteXml(System.Xml.XmlWriter writer);
    }
}
