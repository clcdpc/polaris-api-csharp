using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class HoldAndCirculationIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task HoldRequestCancelAsync_WithNonexistentRequest_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, NonexistentRequestId, Settings.PatronPin, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -4201);
    }

    [TestMethod]
    public async Task HoldRequestCreateAsync_WithNonexistentBib_ReturnsDocumentedError()
    {
        RequirePatronId();

        var holdParams = new HoldRequestCreateParams(Settings.PatronId, NonexistentBibId, BranchIdOrConfiguredOrganizationId, OrganizationIdOrConfigured);
        var response = await Papi.HoldRequestCreateAsync(holdParams);

        PapiIntegrationAssert.PapiError(response, -4006);
    }

    [TestMethod]
    public async Task HoldRequestCreateAsync_ConvenienceOverload_WithNonexistentBib_ReturnsDocumentedError()
    {
        RequirePatronId();

        var response = await ((PapiClient)Papi).HoldRequestCreateAsync(Settings.PatronId, NonexistentBibId, BranchIdOrConfiguredOrganizationId, requestingOrgId: OrganizationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -4006);
    }

    [TestMethod]
    public async Task HoldRequestGetListAsync_WithConfiguredBranch_ReturnsSuccessShape()
    {
        RequirePapiConfiguration();

        var response = await Papi.HoldRequestGetListAsync(BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsNotNull(data.RequestPicklistRows);
    }

    [TestMethod]
    public async Task HoldRequestReactivateAsync_WithNonexistentRequest_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, NonexistentRequestId, DateTime.UtcNow, UserIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -4201);
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
    public async Task HoldRequestSuspendAsync_WithNonexistentRequest_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, NonexistentRequestId, DateTime.UtcNow, Settings.PatronPin, UserIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -4201);
    }

    [TestMethod]
    public async Task UpdatePickupBranchIDAsync_WithNonexistentRequest_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.UpdatePickupBranchIDAsync(Settings.PatronBarcode, NonexistentRequestId, BranchIdOrConfiguredOrganizationId, Settings.PatronPin, UserIdOrConfigured, WorkstationIdOrConfigured);

        PapiIntegrationAssert.PapiError(response, -4201);
    }

    [TestMethod]
    public async Task ItemRenewAsync_WithNonexistentItem_ReturnsDocumentedError()
    {
        RequirePatronCredentials();

        var response = await Papi.ItemRenewAsync(Settings.PatronBarcode, NonexistentItemRecordId, Settings.PatronPin);

        PapiIntegrationAssert.PapiError(response, -6001);
    }

    [TestMethod]
    public void ItemRenewAllForPatronAsync_RequiresScenarioDataAndIsDisabledByDefault()
    {
        RequireScenarioDependentTestsEnabled(
            nameof(Papi.ItemRenewAllForPatronAsync),
            "TestSettings:PatronBarcode and TestSettings:PatronPin for a disposable patron with renewable checked-out items",
            "call ItemRenewAllForPatronAsync and assert a deserialized ItemRenewResultWrapper with either success item rows or documented item-level errors");
    }

    [TestMethod]
    public async Task ItemUpdateBarcodeAsync_WithNonexistentItem_IsGatedAsMutatingReachabilityTest()
    {
        RequireMutatingTestsEnabled();

        var response = await Papi.ItemUpdateBarcodeAsync("PAPI-INTEGRATION-NONEXISTENT-BARCODE", NonexistentItemRecordId, BranchIdOrConfiguredOrganizationId);
        var data = PapiIntegrationAssert.HasPapiData(response);

        Assert.IsTrue(data.PAPIErrorCode < 0, "A nonexistent item/barcode update should reach PAPI and return a documented domain error instead of mutating real data.");
    }
}
