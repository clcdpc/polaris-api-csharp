namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.SystemAdministration
{
    [TestClass]
    public sealed class SortOptionsGetTests : IntegrationTestBase
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
