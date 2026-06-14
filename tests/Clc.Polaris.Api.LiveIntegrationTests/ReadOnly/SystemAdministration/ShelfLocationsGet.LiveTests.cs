namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class ShelfLocationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task ShelfLocationsGetTest()
        {
            var response = await Papi.ShelfLocationsGetAsync(7, TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.ShelfLocationsRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task ShelfLocationsGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.ShelfLocationsGetAsync(branchId, TestContext.CancellationToken);

            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.ShelfLocationsRows);
        }
    }
}
