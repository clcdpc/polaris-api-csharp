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
        public int? BibId { get; set; }
        public int? BranchId { get; set; }
        public int? PickupBranchId { get; set; }
        public int? LocalControlNumber { get; set; }
        public int? RecordSetId { get; set; }
        public int? RecordSetRecordId { get; set; }
        public int? HoldableBibId { get; set; }
        public int? HoldPickupBranchId { get; set; }
        public int? StaffUserId { get; set; }
        public int? StaffWorkstationId { get; set; }
    }
}
