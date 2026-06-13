namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ShelfLocationsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ShelfLocationsGetTest()
        {
            var response = await Papi.ShelfLocationsGetAsync(7, TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.ShelfLocationsRows);
        }
    }
}
