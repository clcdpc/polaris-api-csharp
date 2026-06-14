namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class LimitFiltersGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task LimitFiltersGetTest()
        {
            var response = (await Papi.LimitFiltersGetAsync(cancellationToken: TestContext.CancellationToken)).Data;
            Assert.HasCount(response.PAPIErrorCode, response.LimitFiltersRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task LimitFiltersGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.LimitFiltersGetAsync(branchId, TestContext.CancellationToken);

            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.LimitFiltersRows);
        }
    }
}
