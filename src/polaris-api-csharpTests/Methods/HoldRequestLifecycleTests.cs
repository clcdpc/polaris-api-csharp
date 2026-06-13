namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestLifecycle_CanCreateReadSuspendReactivateAndCancelConfiguredHold()
        {
            var holdableBibId = RequirePositiveSetting(Settings.HoldableBibId, nameof(Settings.HoldableBibId));
            var holdPickupBranchId = RequirePositiveSetting(Settings.HoldPickupBranchId, nameof(Settings.HoldPickupBranchId));
            var requestingBranchId = Settings.BranchId ?? holdPickupBranchId;
            var holdNote = CreateUniqueTestArtifactText("hold", maxLength: 80);

            var createResponse = await Papi.HoldRequestCreateAsync(new HoldRequestCreateParams
            {
                PatronID = Settings.PatronId,
                BibID = holdableBibId,
                PickupOrgID = holdPickupBranchId,
                RequestingOrgID = requestingBranchId,
                UserID = Settings.StaffUserId,
                WorkstationID = Settings.StaffWorkstationId,
                PatronNotes = holdNote,
            }, TestContext.CancellationToken);

            Assert.AreEqual(0, createResponse.Data.PAPIErrorCode);
            Assert.IsNotNull(createResponse.Data.RequestGuid);

            var holdRequests = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(0, holdRequests.Data.PAPIErrorCode);
            var createdHold = holdRequests.Data.PatronHoldRequestsGetRows.SingleOrDefault(row => row.BibID == holdableBibId && row.PickupBranchID == holdPickupBranchId);
            Assert.IsNotNull(createdHold, "Expected the created hold to be returned by PatronHoldRequestsGetAsync.");

            var requestId = createdHold.HoldRequestID;
            var suspendResponse = await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, requestId, DateTime.Today.AddDays(7), Settings.PatronPin, userId: Settings.StaffUserId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, suspendResponse.Data.PAPIErrorCode);

            var reactivateResponse = await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, requestId, DateTime.Today, userId: Settings.StaffUserId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, reactivateResponse.Data.PAPIErrorCode);

            var cancelResponse = await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, requestId, Settings.PatronPin, userId: Settings.StaffUserId, workstationId: Settings.StaffWorkstationId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, cancelResponse.Data.PAPIErrorCode);
        }
    }
}
