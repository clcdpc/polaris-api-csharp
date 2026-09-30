namespace Clc.Polaris.Api.Models
{
    public class PatronCodesGetResult : PapiResponseCommon
    {
        public List<PatronCodeRow> PatronCodesRows { get; set; } = new();

        public override string ToString()
        {
            return string.Join("\r\n", PatronCodesRows?.OrderBy(pc => pc.Description) ?? Enumerable.Empty<PatronCodeRow>());
        }
    }

    public class PatronCodeRow
    {
        public int PatronCodeID { get; set; }
        public string? Description { get; set; }

        public override string ToString()
        {
            return $"{PatronCodeID} - {Description}";
        }
    }
}
