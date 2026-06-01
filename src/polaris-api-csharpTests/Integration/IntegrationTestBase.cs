using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

public abstract class IntegrationTestBase
{
    protected const int NonexistentBibId = 2_147_483_647;
    protected const int NonexistentItemRecordId = 2_147_483_647;
    protected const int NonexistentRequestId = 2_147_483_647;
    protected const int NonexistentRecordSetId = 2_147_483_647;
    protected const int NonexistentTitleListId = 2_147_483_647;
    protected const int NonexistentNotificationId = 2_147_483_647;

    private const string MissingConfigurationMessage = "Integration test configuration is missing. Provide appsettings.Test.json or environment variables to run live PAPI tests.";

    protected IPapiClient Papi { get; private set; } = null!;
    protected PapiSettings PapiSettings { get; private set; } = null!;
    protected IntegrationScenarioSettings Settings { get; private set; } = null!;
    protected IntegrationTestOptions Options { get; private set; } = null!;

    [TestInitialize]
    public void InitializeIntegrationTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        PapiSettings = config.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>() ?? new PapiSettings();
        Settings = LoadIntegrationScenarioSettings(config);
        Options = config.GetSection(nameof(IntegrationTestOptions)).Get<IntegrationTestOptions>() ?? new IntegrationTestOptions();

        Papi = new PapiClient(PapiSettings)
        {
            AllowStaffOverrideRequests = Options.EnableStaffProtectedTests && HasUsableStaffOverrideAccount(PapiSettings.PolarisOverrideAccount)
        };
    }

    internal static IntegrationScenarioSettings LoadIntegrationScenarioSettings(IConfiguration config)
    {
        var settings = config.Get<IntegrationScenarioSettings>() ?? new IntegrationScenarioSettings();
        var testSettingsSection = config.GetSection(IntegrationScenarioSettings.SectionName);

        if (testSettingsSection.Exists())
        {
            testSettingsSection.Bind(settings);
        }

        return settings;
    }

    protected void RequirePapiConfiguration()
    {
        var missing = new List<string>();

        if (!HasValue(PapiSettings.Hostname) || IsPlaceholder(PapiSettings.Hostname)) missing.Add("PapiSettings:Hostname");
        if (!HasValue(PapiSettings.AccessId) || IsPlaceholder(PapiSettings.AccessId)) missing.Add("PapiSettings:AccessId");
        if (!HasValue(PapiSettings.AccessKey) || IsPlaceholder(PapiSettings.AccessKey)) missing.Add("PapiSettings:AccessKey");
        if (PapiSettings.OrganizationId <= 0) missing.Add("PapiSettings:OrganizationId");
        if (PapiSettings.UserId <= 0) missing.Add("PapiSettings:UserId");
        if (PapiSettings.WorkstationId <= 0) missing.Add("PapiSettings:WorkstationId");

        InconclusiveIfMissing(missing);
    }

    protected void RequirePatronCredentials()
    {
        RequirePapiConfiguration();
        InconclusiveIfMissing(MissingScenarioSettings(
            (Settings.PatronBarcode, SettingName(nameof(IntegrationScenarioSettings.PatronBarcode))),
            (Settings.PatronPin, SettingName(nameof(IntegrationScenarioSettings.PatronPin)))));
    }

    protected void RequirePatronId()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.PatronId, SettingName(nameof(IntegrationScenarioSettings.PatronId)), "a patron record that can be safely read by the integration suite");
    }

    protected void RequireBibId() => RequireBibScenario();

    protected void RequireBibScenario()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.BibId, SettingName(nameof(IntegrationScenarioSettings.BibId)), "a bibliographic record that can be safely read by the integration suite");
    }

    protected void RequireBranchId() => RequireBranchScenario();

    protected void RequireBranchScenario()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.BranchId, SettingName(nameof(IntegrationScenarioSettings.BranchId)), "a branch/organization ID that can be safely read by the integration suite");
    }

    protected void RequireRecordSetId()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.RecordSetId, SettingName(nameof(IntegrationScenarioSettings.RecordSetId)), "a real record set intended for scenario-dependent assertions");
    }

    protected void RequireItemRecordId()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.ItemRecordId, SettingName(nameof(IntegrationScenarioSettings.ItemRecordId)), "an item record intended for scenario-dependent assertions");
    }

    protected void RequireItemBarcode()
    {
        RequirePapiConfiguration();
        InconclusiveIfMissing(MissingScenarioSettings((Settings.ItemBarcode, SettingName(nameof(IntegrationScenarioSettings.ItemBarcode)))));
    }

    protected void RequirePatronAccountTransactionId()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.PatronAccountTransactionId, SettingName(nameof(IntegrationScenarioSettings.PatronAccountTransactionId)), "a patron account transaction intended for scenario-dependent assertions");
    }

    protected void RequireHoldRequestId()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.HoldRequestId, SettingName(nameof(IntegrationScenarioSettings.HoldRequestId)), "a hold request intended for scenario-dependent assertions");
    }

    protected void RequireRemoteStorageScenario()
    {
        RequirePapiConfiguration();
        RequirePositiveScenarioId(Settings.RemoteStorageBranchId, SettingName(nameof(IntegrationScenarioSettings.RemoteStorageBranchId)), "a branch with remote-storage activity for the configured date range");
        InconclusiveIfMissing(MissingScenarioSettings(
            (Settings.RemoteStorageStartDate, SettingName(nameof(IntegrationScenarioSettings.RemoteStorageStartDate))),
            (Settings.RemoteStorageEndDate, SettingName(nameof(IntegrationScenarioSettings.RemoteStorageEndDate)))));
    }

    protected void RequireStaffProtectedTestsEnabled()
    {
        RequirePapiConfiguration();

        if (!Options.EnableStaffProtectedTests)
        {
            Assert.Inconclusive("Staff/protected integration tests are disabled. Set IntegrationTestOptions:EnableStaffProtectedTests=true and provide PapiSettings:PolarisOverrideAccount credentials.");
        }

        var account = PapiSettings.PolarisOverrideAccount;
        var missing = new List<string>();
        if (account == null)
        {
            missing.Add("PapiSettings:PolarisOverrideAccount");
        }
        else
        {
            if (!HasValue(account.Domain) || IsPlaceholder(account.Domain)) missing.Add("PapiSettings:PolarisOverrideAccount:Domain");
            if (!HasValue(account.Username) || IsPlaceholder(account.Username)) missing.Add("PapiSettings:PolarisOverrideAccount:Username");
            if (!HasValue(account.Password) || IsPlaceholder(account.Password)) missing.Add("PapiSettings:PolarisOverrideAccount:Password");
        }

        InconclusiveIfMissing(missing);
    }

    protected void RequireMutatingTestsEnabled()
    {
        RequirePapiConfiguration();

        if (!Options.EnableMutatingIntegrationTests)
        {
            Assert.Inconclusive("Mutating integration tests are disabled. Set IntegrationTestOptions:EnableMutatingIntegrationTests=true only for safe disposable test fixtures.");
        }
    }

    protected void DocumentScenarioDependentPlaceholder(string methodName, string whyScenarioDataIsRequired, string requiredSettings, string assertionStrategy)
    {
        Assert.Inconclusive($"{methodName} is a scenario-dependent placeholder and has no executable fixture-backed implementation yet. Reason: {whyScenarioDataIsRequired}. Required settings: {requiredSettings}. Intended assertion strategy: {assertionStrategy}. No live API call was made.");
    }

    protected void RequireScenarioDependentTestsEnabled(string methodName, string requiredSettings, string assertionStrategy)
    {
        if (!Options.EnableScenarioDependentTests)
        {
            Assert.Inconclusive($"{methodName} requires scenario data and is disabled by default. Reason: the endpoint depends on local fixture state that cannot be safely inferred. Required settings: {requiredSettings}. Intended assertion strategy: {assertionStrategy}.");
        }
    }

    protected void RequireAuthenticationFailureTestsEnabled()
    {
        RequirePapiConfiguration();

        if (!Options.EnableAuthenticationFailureTests)
        {
            Assert.Inconclusive("Authentication-failure integration tests are disabled. Set IntegrationTestOptions:EnableAuthenticationFailureTests=true only with disposable credentials where failed login attempts are safe.");
        }
    }

    protected void RequireOrgEmailScenario()
    {
        RequirePapiConfiguration();
        InconclusiveIfMissing(MissingScenarioSettings((Settings.OrgEmail, SettingName(nameof(IntegrationScenarioSettings.OrgEmail)))));
    }

    protected int BranchIdOrConfiguredOrganizationId => Settings.EffectiveBranchId(PapiSettings.OrganizationId);
    protected int OrganizationIdOrConfigured => Settings.EffectiveOrganizationId(PapiSettings.OrganizationId);
    protected int UserIdOrConfigured => Settings.EffectiveUserId(PapiSettings.UserId);
    protected int WorkstationIdOrConfigured => Settings.EffectiveWorkstationId(PapiSettings.WorkstationId);

    private static string SettingName(string propertyName) => $"{IntegrationScenarioSettings.SectionName}:{propertyName}";

    private static void RequirePositiveScenarioId(int value, string settingName, string reason)
    {
        if (value <= 0)
        {
            Assert.Inconclusive($"Scenario data required: set {settingName} to {reason}.");
        }
    }

    private static IReadOnlyCollection<string> MissingScenarioSettings(params (string? Value, string SettingName)[] settings) =>
        settings
            .Where(setting => !HasValue(setting.Value) || IsPlaceholder(setting.Value))
            .Select(setting => setting.SettingName)
            .ToList();

    private static void InconclusiveIfMissing(IReadOnlyCollection<string> missing)
    {
        if (missing.Count > 0)
        {
            Assert.Inconclusive($"{MissingConfigurationMessage} Missing settings: {string.Join(", ", missing)}.");
        }
    }

    private static bool HasUsableStaffOverrideAccount(PolarisUser? account) =>
    account != null &&
    HasValue(account.Domain) &&
    !IsPlaceholder(account.Domain) &&
    HasValue(account.Username) &&
    !IsPlaceholder(account.Username) &&
    HasValue(account.Password) &&
    !IsPlaceholder(account.Password);

    private static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value);

    private static bool IsPlaceholder(string? value) =>
        value?.Contains("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase) == true ||
        string.Equals(value, "https://example.org", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "http://example.org", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "https://example.com", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "http://example.com", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "test@example.org", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "test@example.com", StringComparison.OrdinalIgnoreCase);
}
