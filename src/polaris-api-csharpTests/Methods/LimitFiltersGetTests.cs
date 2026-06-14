namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class LimitFiltersGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task LimitFiltersGetTest()
        {
            var response = (await Papi.LimitFiltersGetAsync(cancellationToken: TestContext.CancellationToken)).Data;
            Assert.HasCount(response.PAPIErrorCode, response.LimitFiltersRows);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task LimitFiltersGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.LimitFiltersGetAsync(branchId, TestContext.CancellationToken);

            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.LimitFiltersRows);
        }
    }
}
