using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class ItemCirculationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task ItemRenew_WithInvalidItem_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.ItemRenewAsync(Settings.PatronBarcode, NonexistentId, Settings.PatronPin);
        PapiIntegrationAssert.ErrorCode(response, -6001);
    }

    [TestMethod]
    public void ItemRenewAllForPatron_RequiresScenarioData()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Client.ItemRenewAllForPatronAsync),
            "a disposable patron with renewable checked-out items",
            "call ItemRenewAllForPatronAsync and assert success plus deserialized renewal rows/blocks");
        Assert.Inconclusive("Scenario-dependent placeholder only; no safe checked-out item fixture is configured by default.");
    }

    [TestMethod]
    public async Task ItemUpdateBarcode_WithInvalidItem_WhenStaffEnabled_ReturnsKnownError()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.ItemUpdateBarcodeAsync("PAPI-INTEGRATION-INVALID", NonexistentId, Settings.EffectiveBranchId);
        PapiIntegrationAssert.ErrorCode(response, -2000);
    }
}
