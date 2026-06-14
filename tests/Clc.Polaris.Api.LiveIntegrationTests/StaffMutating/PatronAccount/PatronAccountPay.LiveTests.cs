namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronAccountPayTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffMutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronAccountPayTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = (await Papi.PatronAccountPayAsync(Settings.PatronBarcode, 1234, .01, PaymentMethod.Cash, note: CreateUniqueTestArtifactText(maxLength: 80), cancellationToken: TestContext.CancellationToken)).Data;
            Assert.AreEqual(-3600, response.PAPIErrorCode);
        }
    }
}
