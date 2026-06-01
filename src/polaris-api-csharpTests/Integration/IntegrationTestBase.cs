using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

public abstract class IntegrationTestBase
{
    protected const int NonexistentBibId = 1234;
    protected const int NonexistentItemRecordId = 1234;
    protected const int NonexistentRequestId = 1234;
    protected const int NonexistentRecordSetId = 1234;
    protected const int NonexistentTitleListId = 1234;
    protected const int NonexistentNotificationId = 1234;

    private const string MissingConfigurationMessage = "Integration test configuration is missing. Provide appsettings.Test.json or environment variables to run live PAPI tests.";

    protected IPapiClient Papi { get; private set; } = null!;
    protected PapiSettings PapiSettings { get; private set; } = null!;
    protected TestSettings Settings { get; private set; } = null!;
    protected IntegrationTestOptions Options { get; private set; } = null!;

    [TestInitialize]
    public void InitializeIntegrationTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        PapiSettings = config.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>() ?? new PapiSettings();
        Settings = config.GetSection(TestSettings.SECTION_NAME).Get<TestSettings>() ?? new TestSettings();
        Options = config.GetSection(nameof(IntegrationTestOptions)).Get<IntegrationTestOptions>() ?? new IntegrationTestOptions();

        Papi = new PapiClient(PapiSettings);
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

        var missing = new List<string>();
        if (!HasValue(Settings.PatronBarcode) || IsPlaceholder(Settings.PatronBarcode)) missing.Add("TestSettings:PatronBarcode");
        if (!HasValue(Settings.PatronPin) || IsPlaceholder(Settings.PatronPin)) missing.Add("TestSettings:PatronPin");

        InconclusiveIfMissing(missing);
    }

    protected void RequirePatronId()
    {
        RequirePapiConfiguration();

        if (Settings.PatronId <= 0)
        {
            Assert.Inconclusive($"{MissingConfigurationMessage} Missing setting: TestSettings:PatronId.");
        }
    }

    protected void RequireBibScenario()
    {
        RequirePapiConfiguration();

        if (Settings.BibId <= 0)
        {
            Assert.Inconclusive($"Scenario data required: set TestSettings:KnownBibId to a bibliographic record that can be safely read.");
        }
    }

    protected void RequireBranchScenario()
    {
        RequirePapiConfiguration();

        if (Settings.BranchId <= 0)
        {
            Assert.Inconclusive($"Scenario data required: set TestSettings:BranchId to a branch/organization ID that can be safely read.");
        }
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
        RequirePatronCredentials();

        if (!Options.EnableMutatingIntegrationTests)
        {
            Assert.Inconclusive("Mutating integration tests are disabled. Set IntegrationTestOptions:EnableMutatingIntegrationTests=true only for safe test fixtures.");
        }
    }

    protected void RequireScenarioDependentTestsEnabled(string methodName, string requiredSettings, string assertionStrategy)
    {
        if (!Options.EnableScenarioDependentTests)
        {
            Assert.Inconclusive($"{methodName} requires scenario data and is disabled by default. Required settings: {requiredSettings}. Intended assertions: {assertionStrategy}.");
        }
    }

    protected void RequireOrgEmailScenario()
    {
        RequirePapiConfiguration();

        if (!HasValue(Settings.OrgEmail) || IsPlaceholder(Settings.OrgEmail))
        {
            Assert.Inconclusive("Scenario data required: set TestSettings:OrgEmail to the expected ORGEMAIL System Administration value for the configured organization.");
        }
    }

    protected int BranchIdOrConfiguredOrganizationId => Settings.BranchId > 0 ? Settings.BranchId : PapiSettings.OrganizationId;
    protected int OrganizationIdOrConfigured => Settings.OrganizationId > 0 ? Settings.OrganizationId : PapiSettings.OrganizationId;
    protected int UserIdOrConfigured => Settings.UserId > 0 ? Settings.UserId : PapiSettings.UserId;
    protected int WorkstationIdOrConfigured => Settings.WorkstationId > 0 ? Settings.WorkstationId : PapiSettings.WorkstationId;

    private static void InconclusiveIfMissing(IReadOnlyCollection<string> missing)
    {
        if (missing.Count > 0)
        {
            Assert.Inconclusive($"{MissingConfigurationMessage} Missing settings: {string.Join(", ", missing)}.");
        }
    }

    private static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value);

    private static bool IsPlaceholder(string? value) =>
        value?.Contains("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase) == true ||
        string.Equals(value, "https://example.org", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "test@example.org", StringComparison.OrdinalIgnoreCase);
}
