using Clc.Polaris.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

public abstract class PapiIntegrationTestBase
{
    private const string MissingPapiConfigurationMessage = "Integration test configuration is missing or still contains placeholders. Provide appsettings.Test.json or environment variables to run live PAPI tests.";

    private PapiClient? _client;
    private PapiSettings? _papiSettings;
    private IntegrationScenarioSettings? _settings;
    private IntegrationTestOptions? _options;

    protected PapiClient Client => _client ??= new PapiClient(RequirePapiSettings());
    protected IntegrationScenarioSettings Settings => _settings ??= LoadScenarioSettings();
    protected IntegrationTestOptions Options => _options ??= Configuration.GetSection(IntegrationTestOptions.SectionName).Get<IntegrationTestOptions>() ?? new IntegrationTestOptions();
    protected IConfiguration Configuration { get; private set; } = null!;

    [TestInitialize]
    public void InitializeIntegrationTest()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        _client = null;
        _papiSettings = null;
        _settings = null;
        _options = null;
    }

    protected PapiSettings RequirePapiSettings()
    {
        _papiSettings ??= Configuration.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>() ?? new PapiSettings();

        var missing = new List<string>();
        AddIfInvalid(missing, nameof(PapiSettings.Hostname), _papiSettings.Hostname);
        AddIfInvalid(missing, nameof(PapiSettings.AccessId), _papiSettings.AccessId);
        AddIfInvalid(missing, nameof(PapiSettings.AccessKey), _papiSettings.AccessKey);

        if (missing.Count > 0)
        {
            Assert.Inconclusive($"{MissingPapiConfigurationMessage} Missing: {string.Join(", ", missing)}.");
        }

        return _papiSettings;
    }

    protected void RequirePatronCredentials()
    {
        RequireScenarioValue(Settings.PatronBarcode, "TestSettings:PatronBarcode");
        RequireScenarioValue(Settings.PatronPin, "TestSettings:PatronPin");
    }

    protected void RequirePatronId()
    {
        RequirePositive(Settings.PatronId, "TestSettings:PatronId");
    }

    protected void RequireBibId()
    {
        RequirePositive(Settings.BibId, "TestSettings:BibId");
    }

    protected void RequireBranchId()
    {
        RequirePositive(Settings.BranchId, "TestSettings:BranchId");
    }

    protected void RequireOrgEmail()
    {
        RequireScenarioValue(Settings.OrgEmail, "TestSettings:OrgEmail");
    }

    protected void RequireRecordSetId()
    {
        RequirePositive(Settings.RecordSetId, "TestSettings:RecordSetId");
    }

    protected void RequireItemRecordId()
    {
        RequirePositive(Settings.ItemRecordId, "TestSettings:ItemRecordId");
    }

    protected void RequireItemBarcode()
    {
        RequireScenarioValue(Settings.ItemBarcode, "TestSettings:ItemBarcode");
    }

    protected void RequirePatronAccountTransactionId()
    {
        RequirePositive(Settings.PatronAccountTransactionId, "TestSettings:PatronAccountTransactionId");
    }

    protected void RequireHoldRequestId()
    {
        RequirePositive(Settings.HoldRequestId, "TestSettings:HoldRequestId");
    }

    protected void RequireStaffProtectedTestsEnabled()
    {
        RequirePapiSettings();
        var account = Client.StaffOverrideAccount;
        if (!Options.EnableStaffProtectedTests)
        {
            Assert.Inconclusive("Staff/protected integration tests are disabled. Set IntegrationTestOptions:EnableStaffProtectedTests=true to run them.");
        }

        if (account == null || IsPlaceholderOrEmpty(account.Domain) || IsPlaceholderOrEmpty(account.Username) || IsPlaceholderOrEmpty(account.Password))
        {
            Assert.Inconclusive("Staff/protected integration tests require PapiSettings:PolarisOverrideAccount:Domain, Username, and Password.");
        }
    }

    protected void RequireMutatingTestsEnabled()
    {
        RequirePapiSettings();
        if (!Options.EnableMutatingIntegrationTests)
        {
            Assert.Inconclusive("Mutating integration tests are disabled. Set IntegrationTestOptions:EnableMutatingIntegrationTests=true only for disposable/safe fixtures.");
        }
    }

    protected void RequireScenarioDependentTestsEnabled(string methodName, string requiredFixture, string assertionStrategy)
    {
        RequirePapiSettings();
        if (!Options.EnableScenarioDependentTests)
        {
            Assert.Inconclusive($"{methodName} requires scenario data ({requiredFixture}). Intended assertion strategy: {assertionStrategy}. Set IntegrationTestOptions:EnableScenarioDependentTests=true after configuring safe fixtures.");
        }
    }

    protected void RequireAuthenticationFailureTestsEnabled()
    {
        RequirePapiSettings();
        if (!Options.EnableAuthenticationFailureTests)
        {
            Assert.Inconclusive("Authentication-failure integration tests are disabled to avoid repeated bad PIN/password attempts against real accounts.");
        }
    }

    protected static int NonexistentId => 1234;
    protected static int NonexistentLargeId => 999_999_999;

    private IntegrationScenarioSettings LoadScenarioSettings()
    {
        var sectionSettings = Configuration.GetSection(IntegrationScenarioSettings.SectionName).Get<IntegrationScenarioSettings>();
        if (sectionSettings != null)
        {
            return sectionSettings;
        }

        // Backward compatibility with the previous flat appsettings.template.json shape.
        return Configuration.Get<IntegrationScenarioSettings>() ?? new IntegrationScenarioSettings();
    }

    private static void RequirePositive(int value, string name)
    {
        if (value <= 0)
        {
            Assert.Inconclusive($"{name} must be configured with a positive, non-placeholder value for this integration test.");
        }
    }

    private static void RequireScenarioValue(string? value, string name)
    {
        if (IsPlaceholderOrEmpty(value))
        {
            Assert.Inconclusive($"{name} must be configured with a non-placeholder value for this integration test.");
        }
    }

    private static void AddIfInvalid(ICollection<string> missing, string name, string? value)
    {
        if (IsPlaceholderOrEmpty(value) || string.Equals(value, "https://example.org", StringComparison.OrdinalIgnoreCase))
        {
            missing.Add(name);
        }
    }

    private static bool IsPlaceholderOrEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            || value.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase)
            || value.Contains("REPLACE_WITH", StringComparison.OrdinalIgnoreCase);
    }
}
