namespace Clc.Polaris.Api
{
    public class TestSettings
    {
        public int PatronId { get; set; }
        public string PatronBarcode { get; set; } = string.Empty;
        public string PatronPin { get; set; } = string.Empty;
        public string FreeTextBlock { get; set; } = string.Empty;
        public string PatronListName { get; set; } = string.Empty;
        public string OrgEmail { get; set; } = string.Empty;
        public int RecordSetId { get; set; }
    }
}
