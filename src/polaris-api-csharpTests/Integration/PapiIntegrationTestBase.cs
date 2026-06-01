using Clc.Polaris.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

public abstract class PapiIntegrationTestBase
{
    protected const string MissingIntegrationConfigurationMessage =
        "Integration test configuration is missing. Provide appsettings.Test.json or environment variables to run integration tests.";

    protected const int NonexistentId = 2_147_483_001;
    protected const string IntegrationNote = "PAPI integration test";

    private static readonly Lazy<IntegrationFixture> LazyFixture = new(IntegrationFixture.Create);

    protected IntegrationFixture Fixture => LazyFixture.Value;
    protected PapiClient Papi => Fixture.RequireClient();
    protected TestSettings Settings => Fixture.TestSettings;
    protected IntegrationTestOptions Options => Fixture.Options;

    protected void RequirePatronCredentials()
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(Settings.PatronBarcode) || IsPlaceholder(Settings.PatronBarcode)) missing.Add("TestSettings:PatronBarcode");
        if (string.IsNullOrWhiteSpace(Settings.PatronPin) || IsPlaceholder(Settings.PatronPin)) missing.Add("TestSettings:PatronPin");
        if (Settings.PatronId <= 0) missing.Add("TestSettings:PatronId");

        InconclusiveIfMissing(missing, "patron success-path fixture data");
    }

    protected void RequireBibScenario()
    {
        var missing = new List<string>();
        if (Settings.BibId <= 0) missing.Add("TestSettings:BibId");
        if (Settings.BranchId <= 0) missing.Add("TestSettings:BranchId");

        InconclusiveIfMissing(missing, "bibliographic fixture data");
    }

    protected int RequireBranchId()
    {
        if (Settings.BranchId <= 0)
        {
            Assert.Inconclusive("TestSettings:BranchId is required for this integration test.");
        }

        return Settings.BranchId;
    }

    protected int RequireOrganizationId()
    {
        var organizationId = Settings.OrganizationId > 0 ? Settings.OrganizationId : Fixture.PapiSettings.OrganizationId;
        if (organizationId <= 0)
        {
            Assert.Inconclusive("TestSettings:OrganizationId or PapiSettings:OrganizationId is required for this integration test.");
        }

        return organizationId;
    }

    protected void RequireStaffProtectedTestsEnabled()
    {
        if (!Options.EnableStaffProtectedTests)
        {
            Assert.Inconclusive("Set IntegrationTestOptions:EnableStaffProtectedTests=true to run staff/protected integration tests.");
        }

        if (Fixture.PapiSettings.PolarisOverrideAccount is null ||
            string.IsNullOrWhiteSpace(Fixture.PapiSettings.PolarisOverrideAccount.Domain) ||
            string.IsNullOrWhiteSpace(Fixture.PapiSettings.PolarisOverrideAccount.Username) ||
            string.IsNullOrWhiteSpace(Fixture.PapiSettings.PolarisOverrideAccount.Password))
        {
            Assert.Inconclusive("PapiSettings:PolarisOverrideAccount credentials are required for staff/protected integration tests.");
        }
    }

    protected void RequireMutatingTestsEnabled()
    {
        if (!Options.EnableMutatingIntegrationTests)
        {
            Assert.Inconclusive("Set IntegrationTestOptions:EnableMutatingIntegrationTests=true to run mutating integration tests.");
        }
    }

    protected void RequireScenarioDependentTestsEnabled(string scenarioDescription)
    {
        if (!Options.EnableScenarioDependentTests)
        {
            Assert.Inconclusive($"Set IntegrationTestOptions:EnableScenarioDependentTests=true and configure {scenarioDescription} to run this scenario-dependent integration test.");
        }
    }

    protected static void InconclusivePlaceholder(string methodName, string reason, string requiredSettings, string assertionStrategy)
    {
        Assert.Inconclusive($"{methodName} requires scenario data before it can be exercised safely. Reason: {reason}. Required settings: {requiredSettings}. Intended assertions: {assertionStrategy}.");
    }

    private static void InconclusiveIfMissing(IReadOnlyCollection<string> missing, string purpose)
    {
        if (missing.Count > 0)
        {
            Assert.Inconclusive($"Missing {purpose}: {string.Join(", ", missing)}.");
        }
    }

    private static bool IsPlaceholder(string value) => value.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase);

    protected sealed class IntegrationFixture
    {
        private IntegrationFixture(PapiSettings papiSettings, TestSettings testSettings, IntegrationTestOptions options)
        {
            PapiSettings = papiSettings;
            TestSettings = testSettings;
            Options = options;
        }

        public PapiSettings PapiSettings { get; }
        public TestSettings TestSettings { get; }
        public IntegrationTestOptions Options { get; }

        public static IntegrationFixture Create()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return new IntegrationFixture(
                configuration.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>() ?? new PapiSettings(),
                configuration.GetSection("TestSettings").Get<TestSettings>() ?? new TestSettings(),
                configuration.GetSection(nameof(IntegrationTestOptions)).Get<IntegrationTestOptions>() ?? new IntegrationTestOptions());
        }

        public PapiClient RequireClient()
        {
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(PapiSettings.Hostname) || PapiSettings.Hostname.StartsWith("https://example", StringComparison.OrdinalIgnoreCase)) missing.Add("PapiSettings:Hostname");
            if (string.IsNullOrWhiteSpace(PapiSettings.AccessId) || PapiSettings.AccessId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase)) missing.Add("PapiSettings:AccessId");
            if (string.IsNullOrWhiteSpace(PapiSettings.AccessKey) || PapiSettings.AccessKey.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase)) missing.Add("PapiSettings:AccessKey");

            if (missing.Count > 0)
            {
                Assert.Inconclusive($"{MissingIntegrationConfigurationMessage} Missing: {string.Join(", ", missing)}.");
            }

            return new PapiClient(PapiSettings);
        }
    }
}
