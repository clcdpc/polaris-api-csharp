namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PickupAreasGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PickupAreasGetAsync_ReturnsRows()
        {
            var response = await Papi.PickupAreasGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PickupAreasRows);
        }
    }
}
