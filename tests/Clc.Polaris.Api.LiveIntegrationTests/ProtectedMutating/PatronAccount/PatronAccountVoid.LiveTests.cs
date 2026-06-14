namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronAccountVoidTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronAccountVoidTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountVoidAsync(Settings.PatronBarcode, 1234, note: CreateUniqueTestArtifactText(maxLength: 80), cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-3606, response.Data.PAPIErrorCode);
        }
    }
}
