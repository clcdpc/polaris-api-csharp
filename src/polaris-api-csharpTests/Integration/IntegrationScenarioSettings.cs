namespace Clc.Polaris.Api.Tests.Integration;

public sealed class IntegrationScenarioSettings
{
    public const string SectionName = "TestSettings";

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
    public int RecordSetId { get; set; }
    public int ItemRecordId { get; set; }
    public string ItemBarcode { get; set; } = string.Empty;
    public int PatronAccountTransactionId { get; set; }
    public int HoldRequestId { get; set; }

    public int EffectiveOrganizationId => OrganizationId > 0 ? OrganizationId : 1;
    public int EffectiveBranchId => BranchId > 0 ? BranchId : EffectiveOrganizationId;
    public int EffectiveWorkstationId => WorkstationId > 0 ? WorkstationId : 1;
    public int EffectiveUserId => UserId > 0 ? UserId : 1;
}
