namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class MultipartGetReadOnlyLiveTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task MultipartGetAsync_ReturnsConfiguredBibAndPatron()
        {
            var bibId = RequirePositiveSetting(Settings.MultipartBibId, nameof(Settings.MultipartBibId));
            var patronId = RequirePositiveSetting(Settings.MultipartPatronId, nameof(Settings.MultipartPatronId));

            var response = await Papi.MultipartGetAsync(bibId, patronId, Settings.MultipartPickupLocationId, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
