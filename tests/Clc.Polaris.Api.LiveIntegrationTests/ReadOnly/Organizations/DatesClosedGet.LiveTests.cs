namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class DatesClosedGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task DatesClosedGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.DatesClosedGetAsync(branchId, TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.IsNotNull(response.Data.DatesClosedRows);
        }
    }
}
