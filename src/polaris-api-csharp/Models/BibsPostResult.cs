using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    [XmlRoot("BibsPostResult")]
    public class BibsPostResult : PapiResponseCommon
    {
        public int? ImportJobID { get; set; }

        public override string ToString() => ImportJobID?.ToString() ?? string.Empty;
    }
}
