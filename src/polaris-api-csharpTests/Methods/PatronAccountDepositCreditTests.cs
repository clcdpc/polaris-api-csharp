namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountDepositCreditTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronAccountDepositCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, note: CreateUniqueTestArtifactText(maxLength: 80), cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
