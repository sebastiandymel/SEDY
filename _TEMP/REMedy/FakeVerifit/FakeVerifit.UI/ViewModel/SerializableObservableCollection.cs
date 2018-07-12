using System.Windows;
using System.Xml.Linq;
using Remedy.CommonUI;

namespace FakeVerifit.UI
{
    public class SerializableFreqObservableCollection : System.Collections.ObjectModel.ObservableCollection<FreqVal>
    {
        public const string CollectionRootName = "FreqValCollection";
        public const string ItemName = "FreqVal";
        public const string FrequencyAttribute = "Frequency";
        public const string ValueAttribute = "Value";

        public string Id { get; set; }

        public XElement Serialize()
        {
            XElement root = new XElement(CollectionRootName);
            root.SetAttributeValue("id", Id);
            foreach (var freqVal in this)
            {
                XElement el = new XElement(ItemName);
                el.SetAttributeValue(FrequencyAttribute, freqVal.Frequency);
                el.SetAttributeValue(ValueAttribute, freqVal.Value);
                root.Add(el);
            }
            return root;
        }

        public void Deserialize(XElement element)
        {
            if (element.Name != CollectionRootName)
            {
                MessageBox.Show($"Bad data source selected to import {Id}");
            }

            var idAttribute = element.Attribute("id");
            if (idAttribute == null || idAttribute.Value != Id)
            {
                MessageBox.Show($"Id not match collection id: {idAttribute?.Value} != {Id}");
            }
            Clear();
            foreach (var el in element.Elements())
            {
                var freqAttribute = el.Attribute(FrequencyAttribute);
                var valueAttribute = el.Attribute(ValueAttribute);
                if (freqAttribute != null && valueAttribute != null)
                {
                    this.Add(new FreqVal
                    {
                        Frequency = int.Parse(freqAttribute.Value),
                        Value = valueAttribute.Value
                    });
                }
            }
        }
    }
}