namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class RecordSetLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedLifecycleLiveTest]
        [DoNotParallelize]
        public async Task RecordSetLifecycle_CanAddAndRemoveConfiguredRecordWithoutChangingInitialMembership()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);
            RequireConfiguredStaffUser();
            RequireConfiguredStaffWorkstation();

            var recordSetId = RequirePositiveSetting(Settings.RecordSetId, nameof(Settings.RecordSetId));
            var recordId = Settings.RecordSetRecordId is > 0 ? Settings.RecordSetRecordId.Value : RequireConfiguredBib();

            var beforeIds = await GetRecordSetIdsAsync(recordSetId);
            var wasPresentBeforeTest = beforeIds.Contains(recordId);

            if (wasPresentBeforeTest)
            {
                await RemoveConfiguredRecordAsync(recordSetId, recordId);
                await AssertRecordSetDoesNotContainRecordAsync(recordSetId, recordId, "Expected configured record to be absent after removal.");

                await AddConfiguredRecordAsync(recordSetId, recordId);
                await AssertRecordSetContainsRecordAsync(recordSetId, recordId, "Expected configured record to be restored because it was present before the test.");

                return;
            }

            await AddConfiguredRecordAsync(recordSetId, recordId);
            await AssertRecordSetContainsRecordAsync(recordSetId, recordId, "Expected configured record to be present after add.");

            await RemoveConfiguredRecordAsync(recordSetId, recordId);
            await AssertRecordSetDoesNotContainRecordAsync(recordSetId, recordId, "Expected configured record to be absent after remove.");
        }

        private async Task AddConfiguredRecordAsync(int recordSetId, int recordId)
        {
            var addResponse = await Papi.RecordSetContentPutAsync(recordSetId, [recordId], RecordSetContentPutActions.Add, Settings.StaffUserId, Settings.StaffWorkstationId, TestContext.CancellationToken);
            Assert.AreEqual(1, addResponse.Data.PAPIErrorCode);
        }

        private async Task RemoveConfiguredRecordAsync(int recordSetId, int recordId)
        {
            var removeResponse = await Papi.RecordSetContentPutAsync(recordSetId, [recordId], RecordSetContentPutActions.Remove, Settings.StaffUserId, Settings.StaffWorkstationId, TestContext.CancellationToken);
            Assert.AreEqual(1, removeResponse.Data.PAPIErrorCode);
        }

        private async Task AssertRecordSetContainsRecordAsync(int recordSetId, int recordId, string failureMessage)
        {
            var ids = await GetRecordSetIdsAsync(recordSetId);
            Assert.Contains(recordId, ids, failureMessage);
        }

        private async Task AssertRecordSetDoesNotContainRecordAsync(int recordSetId, int recordId, string failureMessage)
        {
            var ids = await GetRecordSetIdsAsync(recordSetId);
            Assert.DoesNotContain(recordId, ids, failureMessage);
        }

        private async Task<int[]> GetRecordSetIdsAsync(int recordSetId)
        {
            var response = await Papi.RecordSetRecordsGetAsync(recordSetId, userId: Settings.StaffUserId, workstationId: Settings.StaffWorkstationId, cancellationToken: TestContext.CancellationToken);
            var ids = response.Data.Ids.ToArray();

            Assert.AreEqual(ids.Length, response.Data.PAPIErrorCode);

            return ids;
        }
    }
}