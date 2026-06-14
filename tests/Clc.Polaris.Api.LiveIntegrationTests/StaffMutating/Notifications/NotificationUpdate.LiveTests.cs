namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class NotificationUpdateTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffMutatingLiveTest]
        [DoNotParallelize]
        public async Task NotificationUpdateTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = (await Papi.NotificationUpdateAsync(new NotificationUpdateParams { PatronId = Settings.PatronId, DeliveryString = "test@test.test", ReportingOrgID = 7, NotificationDeliveryDate = DateTime.Now, DeliveryOptionId = 2, Details = CreateUniqueTestArtifactText(maxLength: 80), NotificationStatusId = NotificationStatus.EmailCompleted, NotificationTypeId = 1 }, TestContext.CancellationToken)).Data;
            Assert.AreEqual(-1, response.PAPIErrorCode);
        }
    }
}
