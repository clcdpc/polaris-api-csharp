namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class DatesClosedGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task DatesClosedGetTest()
        {
            var response = await Papi.DatesClosedGetAsync(7, TestContext.CancellationToken);
            Assert.IsNotEmpty(response.Data.DatesClosedRows);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task DatesClosedGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.DatesClosedGetAsync(branchId, TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.IsNotNull(response.Data.DatesClosedRows);
        }
    }
}
