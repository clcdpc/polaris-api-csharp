namespace Clc.Polaris.Api;

public class TestSettings
{
    public int PatronId { get; set; }
    public string PatronBarcode { get; set; } = string.Empty;
    public string PatronPin { get; set; } = string.Empty;
    public int BibId { get; set; }
    public int BranchId { get; set; }
    public int OrganizationId { get; set; }
    public int WorkstationId { get; set; }
    public int UserId { get; set; }
    public string OrgEmail { get; set; } = string.Empty;
    public string PatronListName { get; set; } = string.Empty;
    public string FreeTextBlock { get; set; } = string.Empty;
    public int InvalidBibId { get; set; } = 999_999_999;
    public int InvalidItemRecordId { get; set; } = 999_999_999;
    public int InvalidHoldRequestId { get; set; } = 999_999_999;
    public int InvalidRecordSetId { get; set; } = 999_999_999;
    public int InvalidRecordId { get; set; } = 999_999_999;
    public int InvalidMessageId { get; set; } = 999_999_999;
    public int InvalidPaymentTransactionId { get; set; } = 999_999_999;
    public string BibSearchTerm { get; set; } = "dogs";
    public string BibBooleanSearch { get; set; } = "KW=dogs";
    public string RemoteStorageStartDate { get; set; } = string.Empty;
    public string RemoteStorageEndDate { get; set; } = string.Empty;
    public int RemoteStorageListType { get; set; } = 1;
    public string NewItemBarcode { get; set; } = string.Empty;
    public string OldItemBarcode { get; set; } = string.Empty;
}
