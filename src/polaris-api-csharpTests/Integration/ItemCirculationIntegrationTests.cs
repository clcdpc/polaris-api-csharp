using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class ItemCirculationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task ItemRenew_WithInvalidItem_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.ItemRenewAsync(Settings.PatronBarcode, InvalidItemRecordId, Settings.PatronPin);
        PapiAssert.PapiError(response, -6001);
    }

    [TestMethod]
    public void ItemRenewAllForPatron_RequiresScenarioData()
    {
        RequireScenarioDependentTestsEnabled("ItemRenewAllForPatronAsync requires a test patron with safely renewable checked-out items; assert success or documented per-item renewal errors after fixture setup.");
    }

    [TestMethod]
    public async Task ItemUpdateBarcode_WithInvalidItem_WhenMutatingEnabled_ReturnsKnownPapiError()
    {
        RequireMutatingTestsEnabled();
        RequireStaffProtectedTestsEnabled();
        RequireBranchId();
        RequireNonEmpty(Settings.NewItemBarcode, "TestSettings:NewItemBarcode");

        var response = await Papi.ItemUpdateBarcodeAsync(Settings.NewItemBarcode, InvalidItemRecordId, EffectiveBranchId, Settings.OldItemBarcode);
        PapiAssert.PapiError(response, -2000);
    }
}
