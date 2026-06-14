namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronAccountRefundCreditTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffMutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronAccountRefundCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: CreateUniqueTestArtifactText(maxLength: 80), cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-3606, response.Data.PAPIErrorCode);
        }
    }
}
