using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    public class ILLRequestCancelResult : PapiResponseCommon
    {
        [XmlArray(IsNullable = true)]
        public List<ILLRequestCancelRow>? ILLRequestCancelRows { get; set; }

        public override string ToString() => string.Join("\r\n", ILLRequestCancelRows ?? Enumerable.Empty<ILLRequestCancelRow>());
    }
}
