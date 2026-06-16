namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class BibGetByTypeV2ReadOnlyLiveTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task BibGetByTypeV2Async_ReturnsConfiguredBarcode()
        {
            if (string.IsNullOrWhiteSpace(Settings.BibGetByTypeKey))
            {
                Assert.Inconclusive("Integration test setting 'BibGetByTypeKey' must be configured to run this live scenario.");
            }

            var response = await Papi.BibGetByTypeV2Async(Settings.BibGetByTypeKey, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
