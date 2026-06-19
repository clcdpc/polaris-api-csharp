using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    public class BibGetByTypeResult : PapiResponseCommon
    {
        public List<BibGetByTypeRow> BibGetByTypeRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", BibGetByTypeRows ?? Enumerable.Empty<BibGetByTypeRow>());
    }
}
