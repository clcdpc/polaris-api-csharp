namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class MaterialTypesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task MaterialTypesGetTest()
        {
            var response = await Papi.MaterialTypesGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.MaterialTypesRows);
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task MaterialTypesGetAsync_ReturnsRowsForConfiguredBranch()
        {
            var branchId = RequireConfiguredBranch();

            var response = await Papi.MaterialTypesGetAsync(branchId, TestContext.CancellationToken);

            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.MaterialTypesRows);
        }
    }
}
