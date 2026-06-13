namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountVoidTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronAccountVoidTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountVoidAsync(Settings.PatronBarcode, 1234, note: CreateUniqueTestArtifactText(maxLength: 80), cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-3606, response.Data.PAPIErrorCode);
        }
    }
}
