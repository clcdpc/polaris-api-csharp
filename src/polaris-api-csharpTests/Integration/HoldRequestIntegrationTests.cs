using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class HoldRequestIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task HoldRequestCancel_WithInvalidRequest_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, InvalidHoldRequestId, Settings.PatronPin);
        PapiAssert.PapiError(response, -4201);
    }

    [TestMethod]
    public async Task HoldRequestCreate_WithInvalidBib_ReturnsKnownPapiError()
    {
        RequirePatronId();
        RequireBranchId();

        var response = await Papi.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, InvalidBibId, EffectiveBranchId, EffectiveBranchId));
        PapiAssert.PapiError(response, -4006);
    }

    [TestMethod]
    public async Task HoldRequestCreateOverload_WithInvalidBib_ReturnsKnownPapiError()
    {
        RequirePatronId();
        RequireBranchId();

        var response = await ConcretePapi.HoldRequestCreateAsync(Settings.PatronId, InvalidBibId, EffectiveBranchId);
        PapiAssert.PapiError(response, -4006);
    }

    [TestMethod]
    public async Task HoldRequestGetList_WithConfiguredBranch_ReturnsSuccess()
    {
        RequireBranchId();

        var response = await Papi.HoldRequestGetListAsync(EffectiveBranchId);
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task HoldRequestReactivate_WithInvalidRequest_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, InvalidHoldRequestId, DateTime.UtcNow.Date.AddDays(7));
        PapiAssert.PapiError(response, -4201);
    }

    [TestMethod]
    public async Task HoldRequestReply_WithEmptyGuid_ReturnsKnownPapiError()
    {
        RequireBranchId();

        var response = await Papi.HoldRequestReplyAsync(new HoldRequestCreateResult { RequestGuid = Guid.Empty }, EffectiveBranchId, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);
        PapiAssert.PapiError(response, -4101);
    }

    [TestMethod]
    public async Task HoldRequestSuspend_WithInvalidRequest_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();

        var response = await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, InvalidHoldRequestId, DateTime.UtcNow.Date.AddDays(7), Settings.PatronPin);
        PapiAssert.PapiError(response, -4201);
    }

    [TestMethod]
    public async Task UpdatePickupBranchId_WithInvalidRequest_ReturnsKnownPapiError()
    {
        RequirePatronCredentials();
        RequireBranchId();

        var response = await Papi.UpdatePickupBranchIDAsync(Settings.PatronBarcode, InvalidHoldRequestId, EffectiveBranchId, Settings.PatronPin);
        PapiAssert.PapiError(response, -4201);
    }
}
