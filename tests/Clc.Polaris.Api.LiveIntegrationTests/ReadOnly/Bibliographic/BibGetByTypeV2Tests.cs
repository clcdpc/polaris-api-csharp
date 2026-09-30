namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Bibliographic
{
    [TestClass]
    public sealed class BibGetByTypeV2Tests : IntegrationTestBase
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
