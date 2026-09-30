namespace Clc.Polaris.Api.Models
{
    public class MarcDataField
    {
        public string? Tag { get; set; }
        public string? Ind1 { get; set; }
        public string? Ind2 { get; set; }
        public List<MarcSubfield>? Subfields { get; set; }
    }
}
