using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    public class BibGetByTypeRow
    {
        public int ElementID { get; set; }

        [XmlElement("Occurence")]
        public int Occurrence { get; set; }

        [XmlIgnore]
        public int Occurence
        {
            get => Occurrence;
            set => Occurrence = value;
        }

        public string? Label { get; set; }
        public string? Value { get; set; }
        public bool? Alternate { get; set; }
    }
}
