using Clc.Polaris.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration.Infrastructure;

public abstract class PapiIntegrationTestBase
{
    private const string MissingPapiConfigurationMessage = "Integration test configuration is missing. Copy appsettings.Test.example.json to appsettings.Test.json or provide PapiSettings__* environment variables.";

    protected IPapiClient Papi { get; private set; } = null!;
    protected PapiClient ConcretePapi => (PapiClient)Papi;
    protected PapiSettings PapiSettings { get; private set; } = null!;
    protected TestSettings Settings { get; private set; } = new();
    protected IntegrationTestOptions Options { get; private set; } = new();

    [TestInitialize]
    public void InitializePapiIntegrationTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        try
        {
            PapiSettings = configuration.GetSection(PapiSettings.SECTION_NAME).Get<PapiSettings>() ?? new PapiSettings();
            Settings = configuration.GetSection("TestSettings").Get<TestSettings>() ?? configuration.Get<TestSettings>() ?? new TestSettings();
            Options = configuration.GetSection(nameof(IntegrationTestOptions)).Get<IntegrationTestOptions>() ?? new IntegrationTestOptions();
        }
        catch (InvalidOperationException ex)
        {
            Assert.Inconclusive($"Integration test configuration could not be bound. Check numeric and boolean settings. {ex.Message}");
        }

        if (!HasRequiredPapiSettings(PapiSettings))
        {
            Assert.Inconclusive(MissingPapiConfigurationMessage);
        }

        Papi = new PapiClient(PapiSettings);
    }

    protected void RequirePatronCredentials()
    {
        RequireNonEmpty(Settings.PatronBarcode, "TestSettings:PatronBarcode");
        RequireNonEmpty(Settings.PatronPin, "TestSettings:PatronPin");
    }

    protected void RequirePatronId() => RequirePositive(Settings.PatronId, "TestSettings:PatronId");
    protected void RequireBibId() => RequirePositive(Settings.BibId, "TestSettings:BibId");
    protected void RequireBranchId() => RequirePositive(EffectiveBranchId, "TestSettings:BranchId or PapiSettings:OrganizationId");
    protected void RequireOrganizationId() => RequirePositive(EffectiveOrganizationId, "TestSettings:OrganizationId or PapiSettings:OrganizationId");
    protected void RequireWorkstationId() => RequirePositive(EffectiveWorkstationId, "TestSettings:WorkstationId or PapiSettings:WorkstationId");
    protected void RequireUserId() => RequirePositive(EffectiveUserId, "TestSettings:UserId or PapiSettings:UserId");

    protected void RequireMutatingTestsEnabled()
    {
        if (!Options.EnableMutatingIntegrationTests)
        {
            Assert.Inconclusive("Mutating integration test skipped. Set IntegrationTestOptions__EnableMutatingIntegrationTests=true to opt in.");
        }
    }

    protected void RequireStaffProtectedTestsEnabled()
    {
        if (!Options.EnableStaffProtectedTests)
        {
            Assert.Inconclusive("Staff/protected integration test skipped. Set IntegrationTestOptions__EnableStaffProtectedTests=true and configure PapiSettings__PolarisOverrideAccount__* to opt in.");
        }

        if (Papi.StaffOverrideAccount is null
            || string.IsNullOrWhiteSpace(Papi.StaffOverrideAccount.Domain)
            || string.IsNullOrWhiteSpace(Papi.StaffOverrideAccount.Username)
            || string.IsNullOrWhiteSpace(Papi.StaffOverrideAccount.Password))
        {
            Assert.Inconclusive("Staff/protected integration test skipped because PolarisOverrideAccount is not fully configured.");
        }
    }

    protected void RequireScenarioDependentTestsEnabled(string reason)
    {
        if (!Options.EnableScenarioDependentTests)
        {
            Assert.Inconclusive($"Scenario-dependent integration test skipped. {reason} Set IntegrationTestOptions__EnableScenarioDependentTests=true after configuring required fixture data.");
        }
    }

    protected void RequireAuthenticationFailureTestsEnabled()
    {
        if (!Options.EnableAuthenticationFailureTests)
        {
            Assert.Inconclusive("Authentication-failure integration test skipped to avoid repeated bad PIN/password attempts against live accounts.");
        }
    }

    protected int EffectiveBranchId => Settings.BranchId > 0 ? Settings.BranchId : PapiSettings.OrganizationId;
    protected int EffectiveOrganizationId => Settings.OrganizationId > 0 ? Settings.OrganizationId : PapiSettings.OrganizationId;
    protected int EffectiveWorkstationId => Settings.WorkstationId > 0 ? Settings.WorkstationId : PapiSettings.WorkstationId;
    protected int EffectiveUserId => Settings.UserId > 0 ? Settings.UserId : PapiSettings.UserId;

    protected int InvalidBibId => Settings.InvalidBibId > 0 ? Settings.InvalidBibId : 999_999_999;
    protected int InvalidItemRecordId => Settings.InvalidItemRecordId > 0 ? Settings.InvalidItemRecordId : 999_999_999;
    protected int InvalidHoldRequestId => Settings.InvalidHoldRequestId > 0 ? Settings.InvalidHoldRequestId : 999_999_999;
    protected int InvalidRecordSetId => Settings.InvalidRecordSetId > 0 ? Settings.InvalidRecordSetId : 999_999_999;
    protected int InvalidRecordId => Settings.InvalidRecordId > 0 ? Settings.InvalidRecordId : 999_999_999;
    protected int InvalidMessageId => Settings.InvalidMessageId > 0 ? Settings.InvalidMessageId : 999_999_999;
    protected int InvalidPaymentTransactionId => Settings.InvalidPaymentTransactionId > 0 ? Settings.InvalidPaymentTransactionId : 999_999_999;

    protected static void RequireNonEmpty(string? value, string settingName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Assert.Inconclusive($"Integration test requires {settingName}.");
        }
    }

    protected static void RequirePositive(int value, string settingName)
    {
        if (value <= 0)
        {
            Assert.Inconclusive($"Integration test requires a positive {settingName}.");
        }
    }

    private static bool HasRequiredPapiSettings(PapiSettings settings)
    {
        return !string.IsNullOrWhiteSpace(settings.AccessId)
            && !string.IsNullOrWhiteSpace(settings.AccessKey)
            && !string.IsNullOrWhiteSpace(settings.Hostname);
    }
}
