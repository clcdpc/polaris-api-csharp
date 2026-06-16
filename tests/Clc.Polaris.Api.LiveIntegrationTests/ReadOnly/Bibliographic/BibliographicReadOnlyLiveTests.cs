namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class BibliographicReadOnlyLiveTests : IntegrationTestBase
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

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task MultipartGetAsync_ReturnsConfiguredBibAndPatron()
        {
            var bibId = RequirePositiveSetting(Settings.MultipartBibId, nameof(Settings.MultipartBibId));
            var patronId = RequirePositiveSetting(Settings.MultipartPatronId, nameof(Settings.MultipartPatronId));

            var response = await Papi.MultipartGetAsync(bibId, patronId, Settings.MultipartPickupLocationId, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task HeadingsSearchAsync_ReturnsResponse()
        {
            var response = await Papi.HeadingsSearchAsync(HeadingSearchQualifier.AU, 5, 1, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
