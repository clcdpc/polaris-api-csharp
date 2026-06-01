namespace Clc.Polaris.Api
{
    public class TestSettings
    {
        public const string SECTION_NAME = "TestSettings";

        public int PatronId { get; set; }
        public string PatronBarcode { get; set; } = string.Empty;
        public string PatronPin { get; set; } = string.Empty;
        public int BibId { get; set; }
        public int KnownBibId
        {
            get => BibId;
            set => BibId = value;
        }
        public int BranchId { get; set; }
        public int OrganizationId { get; set; }
        public int WorkstationId { get; set; }
        public int UserId { get; set; }
        public string OrgEmail { get; set; } = string.Empty;
        public string PatronListName { get; set; } = string.Empty;
        public string FreeTextBlock { get; set; } = string.Empty;
        public int RemoteStorageBranchId { get; set; }
        public string RemoteStorageStartDate { get; set; } = string.Empty;
        public string RemoteStorageEndDate { get; set; } = string.Empty;
    }
}
