namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.SystemAdministration
{
    [TestClass]
    public sealed class SysHoldStatusesGetTests : IntegrationTestBase
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
