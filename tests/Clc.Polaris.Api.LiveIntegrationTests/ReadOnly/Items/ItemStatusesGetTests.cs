namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Items
{
    [TestClass]
    public sealed class ItemStatusesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task ItemStatusesGetAsyncTest()
        {
            var response = await Papi.ItemStatusesGetAsync(7, TestContext.CancellationToken);
            Assert.HasCount(response.Data.PAPIErrorCode, response.Data.ItemStatusesRows);
        }
    }
}
