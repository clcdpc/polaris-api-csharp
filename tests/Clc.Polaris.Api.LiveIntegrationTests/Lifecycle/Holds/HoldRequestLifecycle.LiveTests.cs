namespace Clc.Polaris.Api.LiveIntegrationTests
{
	[TestClass]
	public sealed class HoldRequestLifecycleTests : IntegrationTestBase
	{
		private const int CancelledHoldStatusId = 16;

		private static readonly int[] SuccessfulPreflightCancelCodes = [0, -4205, -4300];

		[TestMethod]
		[LifecycleLiveTest]
		[DoNotParallelize]
		public async Task HoldRequestLifecycle_CleansExistingConfiguredHoldThenCreatesSuspendsReactivatesAndCancels()
		{
			RequireConfiguredHoldSettings();

			var holdNote = CreateUniqueTestArtifactText("hold", maxLength: 80);
			var preExistingHoldIds = await CancelExistingConfiguredHoldsAsync();

			var createResponse = await Papi.HoldRequestCreateAsync(CreateConfiguredHoldRequestCreateParams(holdNote), TestContext.CancellationToken);
			Assert.AreEqual(0, createResponse.Data.PAPIErrorCode);

			var createdHold = await GetSingleNewConfiguredHoldAsync(preExistingHoldIds, "Expected exactly one new non-cancelled configured hold after hold creation.");
			var requestId = createdHold.HoldRequestID;

			var suspendResponse = await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, requestId, DateTime.Today.AddDays(7), Settings.PatronPin, userId: Settings.StaffUserId.GetValueOrDefault(), cancellationToken: TestContext.CancellationToken);
			Assert.AreEqual(0, suspendResponse.Data.PAPIErrorCode);

			var reactivateResponse = await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, requestId, DateTime.Today, userId: Settings.StaffUserId.GetValueOrDefault(), cancellationToken: TestContext.CancellationToken);
			Assert.AreEqual(0, reactivateResponse.Data.PAPIErrorCode);

			var cancelResponse = await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, requestId, Settings.PatronPin, userId: Settings.StaffUserId.GetValueOrDefault(), workstationId: Settings.StaffWorkstationId.GetValueOrDefault(), cancellationToken: TestContext.CancellationToken);
			Assert.AreEqual(0, cancelResponse.Data.PAPIErrorCode);

			var createdHoldStillActive = await GetNonCancelledHoldAsync(requestId);
			Assert.IsNull(createdHoldStillActive, $"Expected final cancellation to remove created hold request {requestId} from the non-cancelled hold list.");
		}

		private void RequireConfiguredHoldSettings()
		{
			RequirePositiveSetting(Settings.HoldableBibId, nameof(Settings.HoldableBibId));
			RequirePositiveSetting(Settings.HoldPickupBranchId, nameof(Settings.HoldPickupBranchId));
			RequireConfiguredStaffUser();
			RequireConfiguredStaffWorkstation();
		}

		private HoldRequestCreateParams CreateConfiguredHoldRequestCreateParams(string holdNote)
		{
			var holdPickupBranchId = Settings.HoldPickupBranchId.GetValueOrDefault();

			return new HoldRequestCreateParams
			{
				PatronID = Settings.PatronId,
				BibID = Settings.HoldableBibId.GetValueOrDefault(),
				PickupOrgID = holdPickupBranchId,
				RequestingOrgID = Settings.BranchId ?? holdPickupBranchId,
				UserID = Settings.StaffUserId.GetValueOrDefault(),
				WorkstationID = Settings.StaffWorkstationId.GetValueOrDefault(),
				PatronNotes = holdNote,
			};
		}

		private async Task<int[]> CancelExistingConfiguredHoldsAsync()
		{
			var existingHolds = await GetNonCancelledConfiguredHoldsAsync();
			var existingHoldIds = existingHolds.Select(hold => hold.HoldRequestID).ToArray();

			foreach (var hold in existingHolds)
			{
				var cancelResponse = await Papi.HoldRequestCancelAsync(Settings.PatronBarcode, hold.HoldRequestID, Settings.PatronPin, userId: Settings.StaffUserId.GetValueOrDefault(), workstationId: Settings.StaffWorkstationId.GetValueOrDefault(), cancellationToken: TestContext.CancellationToken);

				Assert.Contains(
					cancelResponse.Data.PAPIErrorCode,
					SuccessfulPreflightCancelCodes,
					$"Expected preflight cancellation of hold request {hold.HoldRequestID} to succeed or report no work to do.");
			}

			return existingHoldIds;
		}

		private async Task<PatronHoldRequestsGetRow> GetSingleNewConfiguredHoldAsync(int[] preExistingHoldIds, string failureMessage)
		{
			var matchingHolds = await GetNonCancelledConfiguredHoldsAsync();
			var newMatchingHolds = matchingHolds.Where(hold => !preExistingHoldIds.Contains(hold.HoldRequestID)).ToArray();

			Assert.HasCount(1, newMatchingHolds, $"{failureMessage}{Environment.NewLine}{FormatHoldDiagnostic(matchingHolds, preExistingHoldIds)}");

			return newMatchingHolds[0];
		}

		private async Task<PatronHoldRequestsGetRow?> GetNonCancelledHoldAsync(int holdRequestId)
		{
			var holdRequests = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin, TestContext.CancellationToken);
			Assert.AreEqual(0, holdRequests.Data.PAPIErrorCode);

			return holdRequests.Data.PatronHoldRequestsGetRows.SingleOrDefault(row => row.HoldRequestID == holdRequestId && row.StatusID != CancelledHoldStatusId);
		}

		private async Task<PatronHoldRequestsGetRow[]> GetNonCancelledConfiguredHoldsAsync()
		{
			var holdRequests = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin, TestContext.CancellationToken);
			Assert.AreEqual(0, holdRequests.Data.PAPIErrorCode);

			return holdRequests.Data.PatronHoldRequestsGetRows
				.Where(row => row.BibID == Settings.HoldableBibId.GetValueOrDefault() && row.PickupBranchID == Settings.HoldPickupBranchId.GetValueOrDefault() && row.StatusID != CancelledHoldStatusId)
				.ToArray();
		}

		private static string FormatHoldDiagnostic(PatronHoldRequestsGetRow[] matchingHolds, int[] preExistingHoldIds)
		{
			var rows = matchingHolds
				.Select(hold => $"HoldRequestID={hold.HoldRequestID}, BibID={hold.BibID}, PickupBranchID={hold.PickupBranchID}, StatusID={hold.StatusID}, StatusDescription='{hold.StatusDescription}', WasPreExisting={preExistingHoldIds.Contains(hold.HoldRequestID)}")
				.ToArray();

			return rows.Length == 0 ? "No non-cancelled matching holds were returned." : $"Non-cancelled matching holds:{Environment.NewLine}{string.Join(Environment.NewLine, rows)}";
		}
	}
}