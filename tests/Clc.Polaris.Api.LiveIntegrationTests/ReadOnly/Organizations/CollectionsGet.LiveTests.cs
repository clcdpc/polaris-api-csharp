namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class CollectionsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task CollectionsGetTest()
        {
            var response = await Papi.CollectionsGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.IsGreaterThan(300, response.Data.PAPIErrorCode);
            Assert.IsGreaterThan(300, response.Data.CollectionsRows.Count);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.CollectionsRows);
        }

        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task CollectionsGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.CollectionsGetAsync(branchId, TestContext.CancellationToken);

            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.CollectionsRows);
        }
    }
}
