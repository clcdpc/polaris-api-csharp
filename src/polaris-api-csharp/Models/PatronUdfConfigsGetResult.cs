namespace Clc.Polaris.Api.Models
{
    public class PatronUdfConfigsGetResult : PapiResponseCommon
    {
        public List<PatronUdfConfigRow> PatronUdfConfigsRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", PatronUdfConfigsRows ?? Enumerable.Empty<PatronUdfConfigRow>());
    }

    public class PatronUdfConfigRow
    {
        public int UDFID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }
        public string? Label { get; set; }
        public bool? Enabled { get; set; }

        public override string ToString() => $"{UDFID} - {OrganizationID} - {Label ?? Description ?? string.Empty} - {Enabled?.ToString() ?? string.Empty}";
    }
}
