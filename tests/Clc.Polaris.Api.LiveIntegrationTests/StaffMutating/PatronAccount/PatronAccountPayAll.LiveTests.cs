namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronAccountPayAllTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffMutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronAccountPayAllTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountPayAllAsync(Settings.PatronBarcode, 999999.99, PaymentMethod.Cash, note: CreateUniqueTestArtifactText(maxLength: 80), cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-3610, response.Data.PAPIErrorCode);
        }
    }
}
