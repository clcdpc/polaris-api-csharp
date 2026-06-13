namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class RecordSetLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task RecordSetLifecycle_CanAddAndRemoveConfiguredRecord()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var recordSetId = RequirePositiveSetting(Settings.RecordSetId, nameof(Settings.RecordSetId));
            var recordId = Settings.RecordSetRecordId is > 0 ? Settings.RecordSetRecordId.Value : RequireConfiguredBib();

            var beforeResponse = await Papi.RecordSetRecordsGetAsync(recordSetId, userId: Settings.StaffUserId, workstationId: Settings.StaffWorkstationId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, beforeResponse.Data.PAPIErrorCode);

            var addResponse = await Papi.RecordSetContentPutAsync(recordSetId, [recordId], RecordSetContentPutActions.Add, Settings.StaffUserId, Settings.StaffWorkstationId, TestContext.CancellationToken);
            Assert.AreEqual(0, addResponse.Data.PAPIErrorCode);

            var afterAddResponse = await Papi.RecordSetRecordsGetAsync(recordSetId, userId: Settings.StaffUserId, workstationId: Settings.StaffWorkstationId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, afterAddResponse.Data.PAPIErrorCode);
            Assert.Contains(recordId, afterAddResponse.Data.Ids.ToArray());

            var removeResponse = await Papi.RecordSetContentPutAsync(recordSetId, [recordId], RecordSetContentPutActions.Remove, Settings.StaffUserId, Settings.StaffWorkstationId, TestContext.CancellationToken);
            Assert.AreEqual(0, removeResponse.Data.PAPIErrorCode);

            var afterRemoveResponse = await Papi.RecordSetRecordsGetAsync(recordSetId, userId: Settings.StaffUserId, workstationId: Settings.StaffWorkstationId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, afterRemoveResponse.Data.PAPIErrorCode);
            Assert.DoesNotContain(recordId, afterRemoveResponse.Data.Ids.ToArray());
        }
    }
}
