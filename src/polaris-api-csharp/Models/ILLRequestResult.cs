using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    [XmlRoot("ILLRequestResult")]
    public class ILLRequestResult
    {
        public int? PAPIErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RequestGUID { get; set; }
        public string? TxnGroupQualifer { get; set; }
        [XmlIgnore]
        public string? TxnGroupQualifier => TxnGroupQualifer;
        public string? TxnQualifier { get; set; }
        public string? StatusType { get; set; }
        public string? StatusValue { get; set; }
        public string? Message { get; set; }

        public override string ToString() => $"{RequestGUID ?? string.Empty} - {StatusType ?? string.Empty} - {StatusValue ?? string.Empty} - {Message ?? string.Empty}";
    }
}
