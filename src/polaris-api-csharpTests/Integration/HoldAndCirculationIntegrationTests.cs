using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class HoldAndCirculationIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public void HoldRequestCancelAsync_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestCancelAsync),
            "canceling a hold request mutates patron hold state and a hard-coded request ID might exist in a live Polaris database",
            "disposable patron credentials plus a configured disposable hold request ID created for this test",
            "call HoldRequestCancelAsync only for the disposable hold request and assert the documented success or domain error without touching pre-existing holds");
    }

    [TestMethod]
    public void HoldRequestCreateAsync_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestCreateAsync),
            "creating a hold request mutates patron hold state and a hard-coded bib ID might exist in a live Polaris database",
            "a disposable patron ID and disposable bibliographic scenario data explicitly approved for hold creation",
            "call HoldRequestCreateAsync with disposable fixture data and assert the documented PAPIErrorCode while cleaning up any created hold");
    }

    [TestMethod]
    public void HoldRequestCreateAsync_ConvenienceOverload_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(PapiClient.HoldRequestCreateAsync),
            "creating a hold request mutates patron hold state and a hard-coded bib ID might exist in a live Polaris database",
            "a disposable patron ID and disposable bibliographic scenario data explicitly approved for hold creation",
            "call the convenience overload with disposable fixture data and assert the documented PAPIErrorCode while cleaning up any created hold");
    }

    [TestMethod]
    public async Task HoldRequestGetListAsync_WithConfiguredBranch_ReturnsSuccessShape()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.HoldRequestGetListAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.RequestPicklistRows);
    }

    [TestMethod]
    public void HoldRequestReactivateAsync_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestReactivateAsync),
            "reactivating a hold request mutates patron hold state and a hard-coded request ID might exist in a live Polaris database",
            "disposable patron credentials plus a configured disposable suspended hold request ID",
            "call HoldRequestReactivateAsync only for the disposable hold and assert the documented response");
    }

    [TestMethod]
    public void HoldRequestReplyAsync_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestReplyAsync),
            "replying to a hold request can mutate hold state and should not be exercised by default with synthetic request identifiers",
            "a disposable hold request fixture with a request GUID that is safe to reply to",
            "call HoldRequestReplyAsync only for the disposable request and assert the documented response");
    }

    [TestMethod]
    public void HoldRequestSuspendAsync_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestSuspendAsync),
            "suspending a hold request mutates patron hold state and a hard-coded request ID might exist in a live Polaris database",
            "disposable patron credentials plus a configured disposable active hold request ID",
            "call HoldRequestSuspendAsync only for the disposable hold and assert the documented response");
    }

    [TestMethod]
    public void UpdatePickupBranchIDAsync_RequiresDisposableHoldFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.UpdatePickupBranchIDAsync),
            "changing pickup branch mutates patron hold state and a hard-coded request ID might exist in a live Polaris database",
            "disposable patron credentials, a configured disposable hold request ID, and a safe pickup branch ID",
            "call UpdatePickupBranchIDAsync only for the disposable hold and assert the documented response");
    }

    [TestMethod]
    public void ItemRenewAsync_RequiresDisposableCheckoutFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.ItemRenewAsync),
            "renewing an item mutates circulation state and a hard-coded item ID might exist in a live Polaris database",
            "disposable patron credentials plus a configured disposable checked-out item ID",
            "call ItemRenewAsync only for the disposable item and assert success or documented item-level errors");
    }

    [TestMethod]
    public void ItemRenewAllForPatronAsync_RequiresScenarioDataAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.ItemRenewAllForPatronAsync),
            "renew-all mutates circulation state for every renewable item on the patron account",
            "TestSettings:PatronBarcode and TestSettings:PatronPin for a disposable patron with renewable checked-out items",
            "call ItemRenewAllForPatronAsync and assert a deserialized ItemRenewResultWrapper with either success item rows or documented item-level errors");
    }

    [TestMethod]
    public void ItemUpdateBarcodeAsync_RequiresDisposableItemFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.ItemUpdateBarcodeAsync),
            "updating an item barcode mutates item data and must not target a hard-coded item ID that might exist in a live Polaris database",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true plus TestSettings:ItemRecordId and TestSettings:ItemBarcode for a disposable item fixture",
            "call ItemUpdateBarcodeAsync only with explicitly configured disposable item data and assert the documented PAPI response");
    }
}
