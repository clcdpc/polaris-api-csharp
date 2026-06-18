namespace Clc.Polaris.Api.Models
{
    public class PatronStatisticalClassesGetResult : PapiResponseCommon
    {
        public List<PatronStatisticalClassRow> PatronStatisticalClassesRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", PatronStatisticalClassesRows ?? Enumerable.Empty<PatronStatisticalClassRow>());
    }

    public class PatronStatisticalClassRow
    {
        public int StatisticalClassID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }

        public override string ToString() => $"{StatisticalClassID} - {OrganizationID} - {Description ?? string.Empty}";
    }
}
