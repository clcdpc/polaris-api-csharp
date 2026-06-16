namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class SysHoldStatusesGetReadOnlyLiveTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task SysHoldStatusesGetAsync_ReturnsResponse()
        {
            var response = await Papi.SysHoldStatusesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
