using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    [XmlRoot("ILLRequestCreateData")]
    public class ILLRequestCreateData
    {
        public int PatronID { get; set; }
        public string? Title { get; set; }
        public int PickupOrgID { get; set; }
        public string? VolumeNumber { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? Edition { get; set; }
        public string? PublicationDate { get; set; }
        public string? ISBN { get; set; }
        public string? ISSN { get; set; }
        public string? LCCN { get; set; }
        public string? ItemType { get; set; }
        public string? MediumType { get; set; }
        public int? WorkstationID { get; set; }
        public int? UserID { get; set; }
        public int? HoldPickupAreaID { get; set; }
    }
}
