namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class SortOptionsGetReadOnlyLiveTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task SortOptionsGetAsync_ReturnsResponse()
        {
            var response = await Papi.SortOptionsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
