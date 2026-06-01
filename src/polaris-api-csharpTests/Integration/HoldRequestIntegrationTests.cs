using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class HoldRequestIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task HoldRequestCancel_WithInvalidRequest_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.HoldRequestCancelAsync(Settings.PatronBarcode, NonexistentId, Settings.PatronPin, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -4201);
    }

    [TestMethod]
    public async Task HoldRequestCreate_WithInvalidBib_ReturnsKnownError()
    {
        RequirePatronId();

        var response = await Client.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, NonexistentId, Settings.EffectiveBranchId, Settings.EffectiveOrganizationId));
        PapiIntegrationAssert.ErrorCode(response, -4006);
    }

    [TestMethod]
    public async Task HoldRequestCreateOverload_WithInvalidBib_ReturnsKnownError()
    {
        RequirePatronId();

        var response = await Client.HoldRequestCreateAsync(Settings.PatronId, NonexistentId, Settings.EffectiveBranchId, userId: Settings.EffectiveUserId, workstationId: Settings.EffectiveWorkstationId, requestingOrgId: Settings.EffectiveOrganizationId);
        PapiIntegrationAssert.ErrorCode(response, -4006);
    }

    [TestMethod]
    public async Task HoldRequestGetList_WhenStaffEnabled_ReturnsSuccess()
    {
        RequireStaffProtectedTestsEnabled();
        RequireBranchId();

        var response = await Client.HoldRequestGetListAsync(Settings.BranchId);
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task HoldRequestReactivate_WithInvalidRequest_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, NonexistentId, DateTime.UtcNow.AddDays(1), Settings.EffectiveUserId);
        PapiIntegrationAssert.ErrorCode(response, -4201);
    }

    [TestMethod]
    public async Task HoldRequestReply_WithEmptyGuid_ReturnsKnownError()
    {
        var hold = new HoldRequestCreateResult { RequestGuid = Guid.Empty };

        var response = await Client.HoldRequestReplyAsync(hold, Settings.EffectiveOrganizationId, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);
        PapiIntegrationAssert.ErrorCode(response, -4101);
    }

    [TestMethod]
    public async Task HoldRequestSuspend_WithInvalidRequest_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.HoldRequestSuspendAsync(Settings.PatronBarcode, NonexistentId, DateTime.UtcNow.AddDays(7), Settings.PatronPin, Settings.EffectiveUserId);
        PapiIntegrationAssert.ErrorCode(response, -4201);
    }

    [TestMethod]
    public async Task UpdatePickupBranchId_WithInvalidRequest_ReturnsKnownError()
    {
        RequirePatronCredentials();

        var response = await Client.UpdatePickupBranchIDAsync(Settings.PatronBarcode, NonexistentId, Settings.EffectiveBranchId, Settings.PatronPin, Settings.EffectiveUserId, Settings.EffectiveWorkstationId);
        PapiIntegrationAssert.ErrorCode(response, -4201);
    }
}
