using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
public sealed class IntegrationTestBaseSettingsTests
{
    [TestMethod]
    public void LoadIntegrationScenarioSettings_WhenTestSettingsSectionMissing_BindsLegacyRootValues()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(IntegrationScenarioSettings.PatronId)] = "123",
                [nameof(IntegrationScenarioSettings.PatronBarcode)] = "legacy-barcode",
                [nameof(IntegrationScenarioSettings.PatronPin)] = "legacy-pin",
                [nameof(IntegrationScenarioSettings.FreeTextBlock)] = "legacy block",
                [nameof(IntegrationScenarioSettings.PatronListName)] = "legacy-list",
                [nameof(IntegrationScenarioSettings.OrgEmail)] = "legacy@example.org"
            })
            .Build();

        var settings = IntegrationTestBase.LoadIntegrationScenarioSettings(config);

        Assert.AreEqual(123, settings.PatronId);
        Assert.AreEqual("legacy-barcode", settings.PatronBarcode);
        Assert.AreEqual("legacy-pin", settings.PatronPin);
        Assert.AreEqual("legacy block", settings.FreeTextBlock);
        Assert.AreEqual("legacy-list", settings.PatronListName);
        Assert.AreEqual("legacy@example.org", settings.OrgEmail);
    }

    [TestMethod]
    public void LoadIntegrationScenarioSettings_WhenTestSettingsSectionExists_PrefersNestedValues()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(IntegrationScenarioSettings.PatronId)] = "123",
                [nameof(IntegrationScenarioSettings.PatronBarcode)] = "legacy-barcode",
                [$"{IntegrationScenarioSettings.SectionName}:{nameof(IntegrationScenarioSettings.PatronId)}"] = "456",
                [$"{IntegrationScenarioSettings.SectionName}:{nameof(IntegrationScenarioSettings.PatronBarcode)}"] = "nested-barcode"
            })
            .Build();

        var settings = IntegrationTestBase.LoadIntegrationScenarioSettings(config);

        Assert.AreEqual(456, settings.PatronId);
        Assert.AreEqual("nested-barcode", settings.PatronBarcode);
    }
}
