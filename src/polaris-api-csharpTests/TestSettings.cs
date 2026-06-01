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
    public string PatronListName { get; set; } = "PAPI integration test list";
    public string FreeTextBlock { get; set; } = "PAPI integration test block";
    public int RecordSetId { get; set; }
    public int ItemRecordId { get; set; }
    public string ItemBarcode { get; set; } = string.Empty;
    public int RequestId { get; set; }
    public int NotificationId { get; set; }
    public int PatronMessageId { get; set; }
    public int TitleListId { get; set; }
}
