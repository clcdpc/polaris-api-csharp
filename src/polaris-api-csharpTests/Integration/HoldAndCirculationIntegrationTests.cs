using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class HoldAndCirculationIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public void HoldRequestCancelAsync_RequiresDisposableHoldScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestCancelAsync),
            "cancelling a hold request mutates patron hold state and must not target a hard-coded request id that might exist",
            "a disposable patron barcode/PIN and a disposable hold request id created specifically for cancellation",
            "enable mutating tests, cancel only the disposable hold request, and assert the documented PAPIErrorCode without deleting pre-existing patron data");
    }

    [TestMethod]
    public void HoldRequestCreateAsync_RequiresDisposableBibScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestCreateAsync),
            "creating a hold mutates patron hold state and must not target a hard-coded bib id that might exist",
            "a disposable patron id, disposable/approved bib id, pickup branch, requesting organization, and cleanup plan for the created hold",
            "enable mutating tests, create a hold for the disposable fixture, assert PAPIErrorCode 0 or another documented success code, then cancel/clean up only the created hold");
    }

    [TestMethod]
    public void HoldRequestCreateAsync_ConvenienceOverload_RequiresDisposableBibScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestCreateAsync),
            "creating a hold through the convenience overload mutates patron hold state and must not target a hard-coded bib id that might exist",
            "a disposable patron id, disposable/approved bib id, pickup branch, requesting organization, and cleanup plan for the created hold",
            "enable mutating tests, create a hold for the disposable fixture through the overload, assert PAPIErrorCode 0 or another documented success code, then cancel/clean up only the created hold");
    }

    [TestMethod]
    public async Task HoldRequestGetListAsync_WithConfiguredBranch_ReturnsSuccessShape()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.HoldRequestGetListAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode >= 0, data.ErrorMessage);
    }

    [TestMethod]
    public void HoldRequestReactivateAsync_RequiresDisposableHoldScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestReactivateAsync),
            "reactivating a hold request mutates patron hold state and must not target a hard-coded request id that might exist",
            "a disposable patron barcode/PIN and a disposable suspended hold request id created specifically for reactivation",
            "enable mutating tests, reactivate only the disposable hold request, and assert the documented PAPIErrorCode without changing pre-existing holds");
    }

    [TestMethod]
    public async Task HoldRequestReplyAsync_WithEmptyGuid_ReturnsDocumentedError()
    {
        RequirePapiConfiguration();

        var hold = new HoldRequestCreateResult { RequestGuid = Guid.Empty };
        var response = await Papi.HoldRequestReplyAsync(hold, OrganizationIdOrConfigured, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);

        PapiIntegrationAssert.PapiError(response, -4101);
    }

    [TestMethod]
    public void HoldRequestSuspendAsync_RequiresDisposableHoldScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.HoldRequestSuspendAsync),
            "suspending a hold request mutates patron hold state and must not target a hard-coded request id that might exist",
            "a disposable patron barcode/PIN and a disposable active hold request id created specifically for suspension",
            "enable mutating tests, suspend only the disposable hold request, and assert the documented PAPIErrorCode without changing pre-existing holds");
    }

    [TestMethod]
    public void UpdatePickupBranchIDAsync_RequiresDisposableHoldScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.UpdatePickupBranchIDAsync),
            "updating a hold pickup branch mutates patron hold state and must not target a hard-coded request id that might exist",
            "a disposable patron barcode/PIN, disposable hold request id, source pickup branch, and target pickup branch",
            "enable mutating tests, update only the disposable hold request, assert the documented PAPIErrorCode, and restore/delete the fixture hold as appropriate");
    }

    [TestMethod]
    public void ItemRenewAsync_RequiresDisposableCheckoutScenarioAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.ItemRenewAsync),
            "renewing an item mutates circulation state and must not target a hard-coded item record id that might exist",
            "a disposable patron barcode/PIN and a checked-out disposable item record id that is safe to renew",
            "enable mutating tests, renew only the disposable checkout fixture, and assert a deserialized ItemRenewResult with documented success or item-level error details");
    }

    [TestMethod]
    public void ItemRenewAllForPatronAsync_RequiresScenarioDataAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.ItemRenewAllForPatronAsync),
            "renew-all mutates every renewable checkout for the configured patron and requires an intentionally prepared patron fixture",
            "a patron with renewable checked-out items and a site-approved renewal policy fixture",
            "call ItemRenewAllForPatronAsync and assert a deserialized ItemRenewResultWrapper with either success item rows or documented item-level errors");
    }

    [TestMethod]
    public void ItemUpdateBarcodeAsync_RequiresDisposableItemFixtureAndIsDisabledByDefault()
    {
        DocumentScenarioDependentPlaceholder(
            nameof(Papi.ItemUpdateBarcodeAsync),
            "updating an item barcode mutates item data and must not target a hard-coded item record id that might exist",
            "IntegrationTestOptions:EnableMutatingIntegrationTests=true plus TestSettings:ItemRecordId and TestSettings:ItemBarcode for a disposable item fixture",
            "call ItemUpdateBarcodeAsync only for the configured disposable item record/barcode and assert the documented success or fixture-specific error response");
    }
}
