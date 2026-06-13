namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class CollectionsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task CollectionsGetTest()
        {
            var response = await Papi.CollectionsGetAsync(cancellationToken: TestContext.CancellationToken);
            Assert.IsGreaterThan(300, response.Data.PAPIErrorCode);
            Assert.IsGreaterThan(300, response.Data.CollectionsRows.Count);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.CollectionsRows);
        }
    }
}
