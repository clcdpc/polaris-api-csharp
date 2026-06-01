namespace Clc.Polaris.Api.Tests.Integration;

public sealed class IntegrationScenarioSettings
{
    public const string SectionName = "TestSettings";

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
    public int RecordSetId { get; set; }
    public int ItemRecordId { get; set; }
    public string ItemBarcode { get; set; } = string.Empty;
    public int PatronAccountTransactionId { get; set; }
    public int HoldRequestId { get; set; }

    public int EffectiveOrganizationId(int configuredOrganizationId) => OrganizationId > 0 ? OrganizationId : configuredOrganizationId;
    public int EffectiveBranchId(int configuredOrganizationId) => BranchId > 0 ? BranchId : configuredOrganizationId;
    public int EffectiveWorkstationId(int configuredWorkstationId) => WorkstationId > 0 ? WorkstationId : configuredWorkstationId;
    public int EffectiveUserId(int configuredUserId) => UserId > 0 ? UserId : configuredUserId;
}
