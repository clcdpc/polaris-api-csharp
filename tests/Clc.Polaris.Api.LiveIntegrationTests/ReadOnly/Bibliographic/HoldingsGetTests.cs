namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Bibliographic
{
    [TestClass]
    public sealed class HoldingsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task HoldingsGetAsync_ReturnsHoldingsForConfiguredBib()
        {
            var bibId = RequireConfiguredBib();

            var response = await Papi.HoldingsGetAsync(bibId, TestContext.CancellationToken);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.BibHoldingsGetRows);
        }
    }
}
